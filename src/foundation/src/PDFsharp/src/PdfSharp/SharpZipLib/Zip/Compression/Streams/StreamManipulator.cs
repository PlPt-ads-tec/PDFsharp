#if !NET_ZIP
// StreamManipulator.cs
//
// Copyright (C) 2001 Mike Krueger
//
// This file was translated from java, it was part of the GNU Classpath
// Copyright (C) 2001 Free Software Foundation, Inc.
//
// This program is free software; you can redistribute it and/or
// modify it under the terms of the GNU General Public License
// as published by the Free Software Foundation; either version 2
// of the License, or (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
//
// Linking this library statically or dynamically with other modules is
// making a combined work based on this library.  Thus, the terms and
// conditions of the GNU General Public License cover the whole
// combination.
// 
// As a special exception, the copyright holders of this library give you
// permission to link this library with independent modules to produce an
// executable, regardless of the license terms of these independent
// modules, and to copy and distribute the resulting executable under
// terms of your choice, provided that you also meet, for each linked
// independent module, the terms and conditions of the license of that
// module.  An independent module is a module which is not derived from
// or based on this library.  If you modify this library, you may extend
// this exception to your version of the library, but you are not
// obligated to do so.  If you do not wish to do so, delete this
// exception statement from your version.

namespace PdfSharp.SharpZipLib.Zip.Compression.Streams
{

    /// <summary>
    /// This class allows us to retrieve a specified number of bits from
    /// the input buffer, as well as copy big byte blocks.
    ///
    /// It uses an int buffer to store up to 31 bits for direct
    /// manipulation.  This guarantees that we can get at least 16 bits,
    /// but we only need at most 15, so this is all safe.
    ///
    /// There are some optimizations in this class, for example, you must
    /// never peek more than 8 bits more than needed, and you must first
    /// peek bits before you may drop them.  This is not a general purpose
    /// class but optimized for the behavior of the Inflater.
    ///
    /// Authors of the original java version: John Leuner, Jochen Hoenicke
    /// </summary>
    class StreamManipulator
    {
#region Constructors
        /// <summary>
        /// Constructs a default StreamManipulator with all buffers empty
        /// </summary>
        public StreamManipulator()
        { }
#endregion

        /// <summary>
        /// Get the next sequence of bits but don't increase input pointer.  bitCount must be
        /// less or equal 16 and if this call succeeds, you must drop
        /// at least n - 8 bits in the next call.
        /// </summary>
        /// <param name="bitCount">The number of bits to peek.</param>
        /// <returns>
        /// the value of the bits, or -1 if not enough bits available.  */
        /// </returns>
        public int PeekBits(int bitCount)
        {
            if (bitsInBuffer_ < bitCount)
            {
                if (windowStart_ == windowEnd_)
                {
                    return -1; // ok
                }
                buffer_ |= (uint)((window_![windowStart_++] & 0xff |
                                 (window_[windowStart_++] & 0xff) << 8) << bitsInBuffer_);
                bitsInBuffer_ += 16;
            }
            return (int)(buffer_ & ((1 << bitCount) - 1));
        }

        /// <summary>
        /// Drops the next n bits from the input.  You should have called PeekBits
        /// with a bigger or equal n before, to make sure that enough bits are in
        /// the bit buffer.
        /// </summary>
        /// <param name="bitCount">The number of bits to drop.</param>
        public void DropBits(int bitCount)
        {
            buffer_ >>= bitCount;
            bitsInBuffer_ -= bitCount;
        }

        /// <summary>
        /// Gets the next n bits and increases input pointer.  This is equivalent
        /// to <see cref="PeekBits"/> followed by <see cref="DropBits"/>, except for correct error handling.
        /// </summary>
        /// <param name="bitCount">The number of bits to retrieve.</param>
        /// <returns>
        /// the value of the bits, or -1 if not enough bits available.
        /// </returns>
        public int GetBits(int bitCount)
        {
            int bits = PeekBits(bitCount);
            if (bits >= 0)
            {
                DropBits(bitCount);
            }
            return bits;
        }

        /// <summary>
        /// Gets the number of bits available in the bit buffer.  This must be
        /// only called when a previous PeekBits() returned -1.
        /// </summary>
        /// <returns>
        /// the number of bits available.
        /// </returns>
        public int AvailableBits => bitsInBuffer_;

        /// <summary>
        /// Gets the number of bytes available.
        /// </summary>
        /// <returns>
        /// The number of bytes available.
        /// </returns>
        public int AvailableBytes => windowEnd_ - windowStart_ + (bitsInBuffer_ >> 3);

        /// <summary>
        /// Skips to the next byte boundary.
        /// </summary>
        public void SkipToByteBoundary()
        {
            buffer_ >>= (bitsInBuffer_ & 7);
            bitsInBuffer_ &= ~7;
        }

        /// <summary>
        /// Returns true when SetInput can be called
        /// </summary>
        public bool IsNeedingInput => windowStart_ == windowEnd_;

        /// <summary>
        /// Copies bytes from input buffer to output buffer starting
        /// at output[offset].  You have to make sure, that the buffer is
        /// byte aligned.  If not enough bytes are available, copies fewer
        /// bytes.
        /// </summary>
        /// <param name="output">
        /// The buffer to copy bytes to.
        /// </param>
        /// <param name="offset">
        /// The offset in the buffer at which copying starts
        /// </param>
        /// <param name="length">
        /// The length to copy, 0 is allowed.
        /// </param>
        /// <returns>
        /// The number of bytes copied, 0 if no bytes were available.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Length is less than zero
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Bit buffer isnt byte aligned
        /// </exception>
        public int CopyBytes(byte[] output, int offset, int length)
        {
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            if ((bitsInBuffer_ & 7) != 0)
            {
                // bits_in_buffer may only be 0 or a multiple of 8
                throw new InvalidOperationException("Bit buffer is not byte aligned!");
            }

            int count = 0;
            while ((bitsInBuffer_ > 0) && (length > 0))
            {
                output[offset++] = (byte)buffer_;
                buffer_ >>= 8;
                bitsInBuffer_ -= 8;
                length--;
                count++;
            }

            if (length == 0)
            {
                return count;
            }

            int avail = windowEnd_ - windowStart_;
            if (length > avail)
            {
                length = avail;
            }
            Array.Copy(window_!, windowStart_, output, offset, length);
            windowStart_ += length;

            if (((windowStart_ - windowEnd_) & 1) != 0)
            {
                // We always want an even number of bytes in input, see peekBits
                buffer_ = (uint)(window_![windowStart_++] & 0xff);
                bitsInBuffer_ = 8;
            }
            return count + length;
        }

        /// <summary>
        /// Resets state and empties internal buffers
        /// </summary>
        public void Reset()
        {
            buffer_ = 0;
            windowStart_ = windowEnd_ = bitsInBuffer_ = 0;
        }

        /// <summary>
        /// Add more input for consumption.
        /// Only call when IsNeedingInput returns true
        /// </summary>
        /// <param name="buffer">data to be input</param>
        /// <param name="offset">offset of first byte of input</param>
        /// <param name="count">number of bytes of input to add.</param>
        public void SetInput(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(offset), "Cannot be negative");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count), "Cannot be negative");
            }

            if (windowStart_ < windowEnd_)
            {
                throw new InvalidOperationException("Old input was not completely processed");
            }

            int end = offset + count;

            // We want to throw an ArrayIndexOutOfBoundsException early.
            // Note the check also handles integer wrap around.
            if ((offset > end) || (end > buffer.Length))
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if ((count & 1) != 0)
            {
                // We always want an even number of bytes in input, see PeekBits
                buffer_ |= (uint)((buffer[offset++] & 0xff) << bitsInBuffer_);
                bitsInBuffer_ += 8;
            }

            window_ = buffer;
            windowStart_ = offset;
            windowEnd_ = end;
        }

#region Instance Fields

        byte[]? window_;
        int windowStart_;
        int windowEnd_;

        uint buffer_;
        int bitsInBuffer_;
#endregion
    }
}
#endif
