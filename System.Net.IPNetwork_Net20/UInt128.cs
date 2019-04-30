
// https://www.codeproject.com/Tips/785014/UInt-Division-Modulus

namespace System.Numerics
{


    public struct BigInteger
        : System.IComparable
            , System.IComparable<BigInteger>
            , System.Collections.Generic.IComparer<BigInteger>
            , System.IEquatable<BigInteger>
    {

        private ulong Low;
        private ulong High;

        public int Sign
        {
            get {
                return 1;
            }
        }



        // return MapUnsignedToSigned<ulong, long>(ulongValue);
        private static long MapULongToLong(ulong ulongValue)
        {
            return unchecked((long)ulongValue + long.MinValue);
        }


        // return MapSignedToUnsigned<long, ulong>(longValue);
        private static ulong MapLongToUlong(long longValue)
        {
            return unchecked((ulong)(longValue - long.MinValue));
        }


        public long LowSigned
        {
            get { return MapULongToLong(this.Low); }
        }


        public long HighSigned
        {
            get { return MapULongToLong(this.High); }
        }


        public static BigInteger FromSignedValues(long low, long high)
        {
            ulong ulow = MapLongToUlong(low);
            ulong uhigh = MapLongToUlong(high);
            
            return new BigInteger(ulow, uhigh);
        }
        
        
        public BigInteger(ulong low, ulong high)
        {
            this.Low = low;
            this.High = high;
        } // End Constructor 


        public BigInteger(byte[] bytes)
        {
            if (bytes.Length != 16)
                throw new System.ArgumentException("bytes must be 16 bytes long. Actual Length: " + bytes.Length.ToString());

            byte[] upperBytes = new byte[8];
            byte[] lowerBytes = new byte[8];

            System.Array.Copy(bytes, 0, lowerBytes, 0, 8);
            System.Array.Copy(bytes, 8, upperBytes, 0, 8);

            this.Low = System.BitConverter.ToUInt64(lowerBytes,0);
            this.High = System.BitConverter.ToUInt64(upperBytes,0);
        } // End Constructor 


        public BigInteger(BigInteger number)
            : this(number.Low, number.High)
        { } // End Constructor 


        public BigInteger(ulong low)
            : this(low, 0)
        { } //End Constructor 


        public BigInteger(uint low)
            : this(low, 0)
        { } //End Constructor 


        public BigInteger(int low)
            : this((uint)low, 0)
        { } //End Constructor 

        public BigInteger(long low)
            : this((ulong)low, 0)
        { } //End Constructor 


        //public BigInteger()
        //    : this(0, 0)
        //{ } // End Constructor 


        private static int CharToInt(char ch, uint radix)
        {
            int n = -1;

            if (ch >= 'A' && ch <= 'Z')
            {
                if (((ch - 'A') + 10) < radix)
                {
                    n = (ch - 'A') + 10;
                }
                else
                {
                    throw new System.InvalidOperationException("Char c ∈ [A, Z] but radix < c");
                }
            }
            else if (ch >= 'a' && ch <= 'z')
            {
                if (((ch - 'a') + 10) < radix)
                {
                    n = (ch - 'a') + 10;
                }
                else
                {
                    throw new System.InvalidOperationException("Char c ∈ [a, z] but radix < c");
                }
            }
            else if (ch >= '0' && ch <= '9')
            {
                if ((ch - '0') < radix)
                {
                    n = (ch - '0');
                }
                else
                {
                    throw new System.InvalidOperationException("Char c ∈ [0, 9] but radix < c");
                }
            }
            else
            {
                throw new System.InvalidOperationException("Completely invalid character");
            }

            return n;
        } // End Function CharToInt 


        public BigInteger(string sz, uint radix)
            : this(0, 0)
        {
            sz = sz.TrimStart(' ', '\t', '0');
            sz = sz.TrimEnd(' ', '\t');

            if (sz[sz.Length - 1] == '-')
            {
                throw new System.Exception("BigInteger doesn't allow negative numbers.");
            } // End if (sz[sz.Length-1] == '-')

            BigInteger value = new BigInteger();

            uint j = 0;
            for (int i = sz.Length - 1; i > -1; --i)
            {
                int d = CharToInt(sz[i], radix);
                BigInteger digitValue = d * Power(radix, j);
                value += digitValue;
                j++;
            } // Next i 

            this.Low = value.Low;
            this.High = value.High;
        } // End Constructor 


        public BigInteger(string sz)
            : this(sz, 10)
        {
        }


        public static BigInteger MaxValue
        {
            get { return new BigInteger(ulong.MaxValue, ulong.MaxValue); }
        }


        public static BigInteger MinValue
        {
            get { return new BigInteger(ulong.MinValue, ulong.MinValue); }
        }


        // http://stackoverflow.com/questions/11656241/how-to-print-BigInteger-t-number-using-gcc#answer-11659521
        public override string ToString()
        {
            uint[] buf = new uint[40];

            uint i, j, m = 39;
            for (i = 64; i-- > 0;)
            {
                int usi = (int)i;
                // BigInteger n = value;
                // int carry = !!(n & ((BigInteger)1 << i));
                ulong carry = (this.High & (1UL << usi));
                carry = carry != 0 ? 1UL : 0UL; // ToBool
                carry = carry == 0 ? 1UL : 0UL; // ! 
                carry = carry == 0 ? 1UL : 0UL; // ! 

                for (j = 39; j-- > m + 1 || carry != 0;)
                {
                    ulong d = 2 * buf[j] + carry;
                    carry = d > 9 ? 1UL : 0UL;
                    buf[j] = carry != 0 ? (char)(d - 10) : (char)d;
                } // Next j 

                m = j;
            } // Next i 

            for (i = 64; i-- > 0;)
            {
                int usi = (int)i;
                ulong carry = (this.Low & (1UL << usi));
                carry = carry != 0 ? 1UL : 0UL; // ToBool
                carry = carry == 0 ? 1UL : 0UL; // ! 
                carry = carry == 0 ? 1UL : 0UL; // ! 

                for (j = 39; j-- > m + 1 || carry != 0;)
                {
                    ulong d = 2 * buf[j] + carry;
                    carry = d > 9 ? 1UL : 0UL;
                    buf[j] = carry != 0 ? (char)(d - 10) : (char)d;
                } // Next j 

                m = j;
            } // Next i 

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            bool hasFirstNonNull = false;
            for (int k = 0; k < buf.Length - 1; ++k)
            {

                if (hasFirstNonNull || buf[k] != 0)
                {
                    hasFirstNonNull = true;
                    sb.Append(buf[k].ToString());
                } // End if(hasFirstNonNull || buf[k] != 0)

            } // Next k 

            if (sb.Length == 0)
                sb.Append('0');

            string s = sb.ToString();
            sb.Length = 0;
            sb = null;
            return s;
        } // End Function ToString 


        public string ToString(string format)
        {
            return ToAnyBase(16);
        }


        public string ToAnyBase(ulong @base)
        {
            BigInteger num = new BigInteger(this);

            string retValue = null;
            string latinBase = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            if ((int)@base > latinBase.Length)
                throw new System.ArgumentException("Base value not supported.");

            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            do
            {
                char c = latinBase[(int)(num % @base)];
                sb.Insert(0, c);
                num = num / @base;
            } while (num > 0);

            retValue = sb.ToString();
            sb.Length = 0;
            sb = null;

            return retValue;
        }


        public string ToLogicalGuidString(bool ascending)
        {
            ulong @base = 16;
            BigInteger num = new BigInteger(this);

            string retValue = null;
            string latinBase = "0123456789ABCDEF";

            char[] result = new char[36];


            if (ascending)
            {
                for (int i = 35; i > -1; --i)
                {
                    if (i == 8 || i == 13 || i == 18 || i == 23)
                    {
                        result[i] = '-';
                        continue;
                    } // End if 

                    result[i] = latinBase[(int)(num % @base)];
                    num = num / @base;
                } // Next i 
            }
            else
            {
                string hexString = ToAnyBase(16);
                int count = 0;
                for (int i = 0; i < 36; ++i)
                {
                    if (i == 8 || i == 13 || i == 18 || i == 23)
                    {
                        result[i] = '-';
                        continue;
                    } // End if 

                    if (count < hexString.Length)
                        result[i] = hexString[count];
                    else
                        result[i] = '0';

                    count++;
                } // Next i 

            }

            ////for (int i = 35; i > -1; --i)
            //// for (int i = 0; i < 36; ++i)
            //{
            //    if (i == 8 || i == 13 || i == 18 || i == 23)
            //    {
            //        result[i] = '-';
            //        continue;
            //    }

            //    result[i] = latinBase[(int)(num % @base)];
            //    num = num / @base;
            //} // Next i 

            retValue = new string(result);
            result = null;

            return retValue;
        }


        public string ToLogicalGuidString()
        {
            return this.ToLogicalGuidString(true);
        }


        public System.Guid ToLogicalGuid(bool ascending)
        {
            return new System.Guid(this.ToLogicalGuidString(ascending));
        }


        public System.Guid ToLogicalGuid()
        {
            return this.ToLogicalGuid(true);
        }


        public byte[] ToByteArray()
        {
            byte[] bytes = new byte[16];
            byte[] upperBytes = System.BitConverter.GetBytes(this.High);
            byte[] lowerBytes = System.BitConverter.GetBytes(this.Low);

            // System.Array.Copy(upperBytes, 0, bytes, 0, 8);
            // System.Array.Copy(lowerBytes, 0, bytes, 8, 8);

            // Low to High - should be exactly as ToByteArrayLowToHighEnsured
            System.Array.Copy(lowerBytes, 0, bytes, 0, 8);
            System.Array.Copy(upperBytes, 0, bytes, 8, 8);

            upperBytes = null;
            lowerBytes = null;

            return bytes;
        }


        public byte[] ToByteArrayLowToHighEnsured()
        {
            BigInteger num = new BigInteger(this);
            uint @base = 256;

            byte[] lowToHigh = new byte[16];
            int i = 0;
            do
            {
                lowToHigh[i] = (byte)(num % @base);
                num = num / @base;
                ++i;
            } while (num > 0);

            for (; i < 16; ++i)
            {
                lowToHigh[i] = 0;
            } // Next i 

            return lowToHigh;
        } // End Function ToByteArrayLowToHighEnsured 


        public byte[] ToGuidBytes()
        {
            // byte[] ba = this.ToByteArrayLowToHighEnsured();
            byte[] ba = this.ToByteArray(); // Fast 

            int[] guidByteOrder = new int[16] // 16 Bytes = 128 Bit 
               {10, 11, 12, 13, 14, 15,  8,  9,  6,  7,  4,  5,  0,  1,  2,  3};
            // {00, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15}

            byte[] guidBytes = new byte[16];
            for (int i = 0; i < 16; ++i)
            {
                guidBytes[guidByteOrder[15 - i]] = ba[i];
            } // Next i 
            guidByteOrder = null;
            ba = null;

            return guidBytes;
        } // End Function ToGuidBytes 


        public System.Guid ToGuid()
        {
            return new System.Guid(this.ToGuidBytes());
        }
        

        public static BigInteger FromGuid(System.Guid uid)
        {
            byte[] ba = uid.ToByteArray();

            int[] guidByteOrder = new int[16] // 16 Bytes = 128 Bit 
               {10, 11, 12, 13, 14, 15,  8,  9,  6,  7,  4,  5,  0,  1,  2,  3};
            // {00, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15}

            // Low to High - should be exactly as ToByteArrayLowToHighEnsured
            byte[] bigintBytes = new byte[16];
            for (int i = 0; i < 16; ++i)
            {
                bigintBytes[i] = ba[guidByteOrder[15 - i]];
            } // Next i 

            guidByteOrder = null;
            ba = null;

            // byte[] upperBytes = new byte[8];
            // byte[] lowerBytes = new byte[8];
            // System.Array.Copy(bigintBytes, 0, lowerBytes, 0, 8);
            // System.Array.Copy(bigintBytes, 8, upperBytes, 0, 8);

            ulong lower = System.BitConverter.ToUInt64(bigintBytes, 0);
            ulong upper = System.BitConverter.ToUInt64(bigintBytes, 8);
            bigintBytes = null;

            BigInteger reconstruct = new BigInteger(lower, upper);
            return reconstruct;
        } // End Function FromGuid 


        public static BigInteger FromGuid(string guid)
        {
            System.Guid uid = new System.Guid(guid);
            return FromGuid(uid);
        } // End Function FromGuid 


        public static BigInteger FromRandom(System.Random rand)
        {
            BigInteger retVal = new BigInteger();

            byte[] bytes = new byte[16];
            rand.NextBytes(bytes);
            ulong lower = System.BitConverter.ToUInt64(bytes, 0);
            ulong upper = System.BitConverter.ToUInt64(bytes, 8);
            bytes = null;

            retVal = new BigInteger(lower, upper);

            return retVal;
        } // End Function FromGuid 


        public static BigInteger FromRandom()
        {
            System.Random provider = new System.Random();
            BigInteger retVal = FromRandom(provider);
            
            return retVal;
        }


        public static BigInteger FromSecureRandom(
            System.Security.Cryptography.RandomNumberGenerator provider)
        {
            BigInteger retVal = new BigInteger();

            byte[] bytes = new byte[16];
            provider.GetBytes(bytes);
            ulong lower = System.BitConverter.ToUInt64(bytes, 0);
            ulong upper = System.BitConverter.ToUInt64(bytes, 8);
            bytes = null;

            retVal = new BigInteger(lower, upper);

            return retVal;
        } // End Function FromGuid 


        public static BigInteger FromSecureRandom()
        {
            BigInteger retVal = new BigInteger();

            System.Security.Cryptography.RNGCryptoServiceProvider provider = new System.Security.Cryptography.RNGCryptoServiceProvider();
            //using (System.Security.Cryptography.RandomNumberGenerator provider = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                retVal = FromSecureRandom(provider);
            } // End Using provider

            return retVal;
        }


        public static System.Guid SecureRandomGuid()
        {
            BigInteger nummy = FromSecureRandom();

            return nummy.ToGuid();
        }


        public string ToIpV6()
        {
            string ipString = "";
            //we display in total 4 parts for every long

            ulong crtLong = this.Low;
            for (int i = 0; i < 4; ++i)
            {
                ipString = (crtLong & 0xFFFF).ToString("x04") + (ipString == string.Empty ? "" : ":" + ipString);
                crtLong = crtLong >> 16;
            } // Next j 

            crtLong = this.High;
            for (int i = 0; i < 4; ++i)
            {
                ipString = (crtLong & 0xFFFF).ToString("x04") + (ipString == string.Empty ? "" : ":" + ipString);
                crtLong = crtLong >> 16;
            } // Next j 
            return ipString;
        } // End Function ToIpV6 



        public static BigInteger Pow(int a, int b)
        {
            BigInteger aa = new BigInteger(a);
            BigInteger bb = new BigInteger(b);


            BigInteger initialValue = new BigInteger(1);

            if (b == 0)
                return initialValue;


            for (int i = 0; i < b; ++i)
            {
                initialValue *= aa;
            }

            return initialValue;
        }
        

        public static BigInteger operator ++(BigInteger value)
        {
            return Add(value, new BigInteger(1));
        }



        public static BigInteger operator +(BigInteger left, BigInteger right)
        {
            return Add(left, right);
        }

        public static BigInteger operator +(ulong left, BigInteger right)
        {
            return Add(left, right);
        }

        public static BigInteger operator +(uint left, BigInteger right)
        {
            return Add(left, right);
        }

        public static BigInteger operator +(BigInteger left, ulong right)
        {
            return Add(left, right);
        }

        public static BigInteger operator +(BigInteger left, uint right)
        {
            return Add(left, right);
        }


        public static BigInteger operator -(BigInteger left, BigInteger right)
        {
            return Subtract(left, right);
        }


        public static BigInteger operator -(ulong left, BigInteger right)
        {
            return Subtract(left, right);
        }


        public static BigInteger operator -(uint left, BigInteger right)
        {
            return Subtract(left, right);
        }


        public static BigInteger operator -(BigInteger left, ulong right)
        {
            return Subtract(left, right);
        }


        public static BigInteger operator -(BigInteger left, uint right)
        {
            return Subtract(left, right);
        }

        public static BigInteger operator *(BigInteger left, BigInteger right)
        {
            return Multiply(left, right);
        }


        public static BigInteger operator *(BigInteger left, ulong right)
        {
            return Multiply(left, right);
        }


        public static BigInteger operator *(ulong left, BigInteger right)
        {
            return Multiply(left, right);
        }

        public static BigInteger operator *(BigInteger left, uint right)
        {
            return Multiply(left, right);
        }

        public static BigInteger operator *(int left, BigInteger right)
        {
            return Multiply((uint)left, right);
        }

        public static BigInteger operator *(long left, BigInteger right)
        {
            return Multiply((ulong)left, right);
        }


        public static BigInteger operator *(uint left, BigInteger right)
        {
            return Multiply(left, right);
        }


        public static BigInteger operator /(BigInteger dividend, BigInteger divisor)
        {
            return Div(dividend, divisor);
        }

        public static BigInteger operator /(BigInteger dividend, ulong divisor)
        {
            return Div(dividend, divisor);
        }

        public static BigInteger operator /(BigInteger dividend, uint divisor)
        {
            return Div(dividend, divisor);
        }


        public static BigInteger operator %(BigInteger dividend, BigInteger divisor)
        {
            return Mod(dividend, divisor);
        }

        public static BigInteger operator %(BigInteger dividend, ulong divisor)
        {
            return Mod(dividend, divisor);
        }

        public static BigInteger operator %(BigInteger dividend, uint divisor)
        {
            return Mod(dividend, divisor);
        }


        public static BigInteger operator >>(BigInteger value, int shift)
        {
            return ShiftLeft(value, (uint)shift);
        }


        public static BigInteger operator <<(BigInteger value, int shift)
        {
            return ShiftRight(value, (uint)shift);
        }


        public static BigInteger operator &(BigInteger left, BigInteger right)
        {
            return And(left, right);
        }


        public static BigInteger operator |(BigInteger left, BigInteger right)
        {
            return Or(left, right);
        }


        public static BigInteger operator ^(BigInteger left, BigInteger right)
        {
            return XOR(left, right);
        }


        public static BigInteger operator ~(BigInteger left)
        {
            return Not(left);
        }


        public static bool operator ==(BigInteger lhs, BigInteger rhs)
        {
            return (lhs.High == rhs.High && lhs.Low == rhs.Low);
        }


        public static bool operator !=(BigInteger lhs, BigInteger rhs)
        {
            return !(lhs == rhs);
        }


        public static bool operator ==(BigInteger lhs, ulong rhs)
        {
            if (lhs.High != 0)
                return false;

            return (lhs.Low == rhs);
        }


        public static bool operator !=(BigInteger lhs, ulong rhs)
        {
            if (lhs.High != 0)
                return true;

            return (lhs.Low != rhs);
        }


        public static bool operator <(BigInteger lhs, BigInteger rhs)
        {
            if (lhs.High < rhs.High)
                return true;

            return (lhs.High == rhs.High && lhs.Low < rhs.Low);
        }


        public static bool operator >(BigInteger lhs, BigInteger rhs)
        {
            if (lhs.High > rhs.High)
                return true;

            return (lhs.High == rhs.High && lhs.Low > rhs.Low);
        }


        public static bool operator <=(BigInteger lhs, BigInteger rhs)
        {
            if (lhs.High < rhs.High)
                return true;

            if (lhs.High == rhs.High)
            {
                if (lhs.Low <= rhs.Low)
                    return true;
            }

            return false;
        }


        public static bool operator >=(BigInteger lhs, BigInteger rhs)
        {
            if (lhs.High > rhs.High)
                return true;

            if (lhs.High == rhs.High)
            {
                if (lhs.Low >= rhs.Low)
                    return true;
            } // End if (lhs.High == rhs.High)

            return false;
        }

        // User-defined conversion from BigInteger to int 
        public static explicit operator int(BigInteger num)
        {
            return (int)num.Low;
        }


        // User-defined conversion from BigInteger to byte 
        public static explicit operator byte(BigInteger num)
        {
            return (byte)num.Low;
        }


        // User-defined conversion from BigInteger to uint 
        public static explicit operator uint(BigInteger num)
        {
            return (uint)num.Low;
        }


        // User-defined conversion from BigInteger to long 
        public static explicit operator long(BigInteger num)
        {
            return (long)num.Low;
        }


        // User-defined conversion from BigInteger to ulong 
        public static explicit operator ulong(BigInteger num)
        {
            return num.Low;
        }


        // User-defined conversion from uint to BigInteger 
        public static implicit operator BigInteger(uint num)
        {
            return new BigInteger(num);
        }

        // User-defined conversion from ulong to BigInteger 
        public static explicit operator BigInteger(int num)
        {
            return new BigInteger((uint)num);
        }

        // User-defined conversion from ulong to BigInteger 
        public static implicit operator BigInteger(ulong num)
        {
            return new BigInteger(num);
        }

        // User-defined conversion from long to BigInteger 
        public static explicit operator BigInteger(long num)
        {
            return new BigInteger((ulong)num);
        }




        int CompareTo(object obj)
        {
            if (obj == null)
                return 1; // https://msdn.microsoft.com/en-us/library/system.icomparable.compareto(v=vs.110).aspx

            System.Type t = obj.GetType();

            if (object.ReferenceEquals(t, typeof(BigInteger)))
            {
                BigInteger ui = (BigInteger)obj;
                return compare128(this, ui);
            } // End if (object.ReferenceEquals(t, typeof(BigInteger)))

            if (object.ReferenceEquals(t, typeof(System.DBNull)))
                return 1;

            ulong? lowerPart = obj as ulong?;
            if (!lowerPart.HasValue)
                return 1;

            BigInteger compareTarget = new BigInteger(lowerPart.Value, 0);
            return compare128(this, compareTarget);
        } // End Function CompareTo(object obj)


        int System.IComparable.CompareTo(object obj)
        {
            return this.CompareTo(obj);
        }


        public int CompareTo(BigInteger other)
        {
            return compare128(this, other);
        }


        int System.IComparable<BigInteger>.CompareTo(BigInteger other)
        {
            return compare128(this, other);
        }



        int Compare(BigInteger x, BigInteger y)
        {
            return compare128(x, y);
        }


        int System.Collections.Generic.IComparer<BigInteger>.Compare(BigInteger x, BigInteger y)
        {
            return compare128(x, y);
        }


        public bool Equals(BigInteger other)
        {
            return this.High == other.High && this.Low == other.Low;
        }


        bool System.IEquatable<BigInteger>.Equals(BigInteger other)
        {
            return this.Equals(other);
        }




        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param>
        public override bool Equals(object obj)
        {
            if (object.ReferenceEquals(null, obj))
                return false;

            return obj is BigInteger && this.Equals((BigInteger)obj);
        }


        public override int GetHashCode()
        {
            // return 37 * this.High.GetHashCode() + this.Low.GetHashCode();
            return this.Low.GetHashCode() ^ this.High.GetHashCode();
        }


        public static BigInteger Add(BigInteger N, BigInteger M)
        {
            BigInteger A = new BigInteger();
            add128(N, M, ref A);
            return A;
        }


        public static BigInteger Subtract(BigInteger N, BigInteger M)
        {
            BigInteger A = new BigInteger();
            sub128(N, M, ref A);
            return A;
        }


        public static BigInteger Multiply(BigInteger N, BigInteger M)
        {
            BigInteger Ans = new BigInteger();
            mult128(N, M, ref Ans);
            return Ans;
        }


        public static BigInteger Square(BigInteger R)
        {
            BigInteger Ans = new BigInteger();

            sqr128(R, ref Ans);
            return Ans;
        }


        public BigInteger Power(BigInteger @base, uint power)
        {
            BigInteger num = new BigInteger(1);

            for (int i = 0; i < power; ++i)
            {
                num = num * @base;
            }

            return num;
        }


        public static BigInteger Div(BigInteger M, BigInteger N)
        {
            BigInteger Q = new BigInteger();
            div128(M, N, ref Q);
            return Q;
        }


        public static BigInteger Mod(BigInteger M, BigInteger N)
        {
            BigInteger R = new BigInteger();
            mod128(M, N, ref R);
            return R;
        }

        //public class ValueTuple<T1, T2>
        //{
        //    public ValueTuple(T1 a, T2 b)
        //    { }
        //}


        //public static ValueTuple<BigInteger, BigInteger> DivMod(BigInteger M, BigInteger N)
        //{
        //    BigInteger Q = new BigInteger();
        //    BigInteger R = new BigInteger();
        //    bindivmod128(M, N, ref Q, ref R);

        //    return new ValueTuple<BigInteger, BigInteger> (Q, R);
        //}



        public static BigInteger Not(BigInteger N)
        {
            BigInteger A = new BigInteger();
            not128(N, ref A);
            return A;
        }


        public static BigInteger Or(BigInteger N1, BigInteger N2)
        {
            BigInteger A = new BigInteger();
            or128(N1, N2, ref A);
            return A;
        }


        public static BigInteger And(BigInteger N1, BigInteger N2)
        {
            BigInteger A = new BigInteger();
            and128(N1, N2, ref A);
            return A;
        }


        public static BigInteger XOR(BigInteger N1, BigInteger N2)
        {
            BigInteger A = new BigInteger();
            xor128(N1, N2, ref A);
            return A;
        }


        public static BigInteger ShiftLeft(BigInteger N, uint S)
        {
            BigInteger A = new BigInteger();
            shiftleft128(N, S, ref A);
            return A;
        }


        public static BigInteger ShiftRight(BigInteger N, uint S)
        {
            BigInteger A = new BigInteger();
            shiftright128(N, S, ref A);
            return A;
        }


        private static void inc128(BigInteger N, ref BigInteger A)
        {
            A.Low = (N.Low + 1);
            A.High = N.High + (((N.Low ^ A.Low) & N.Low) >> 63);
        }


        private static void dec128(BigInteger N, ref BigInteger A)
        {
            A.Low = N.Low - 1;
            A.High = N.High - (((A.Low ^ N.Low) & A.Low) >> 63);
        }


        private static void add128(BigInteger N, BigInteger M, ref BigInteger A)
        {
            ulong C = (((N.Low & M.Low) & 1) + (N.Low >> 1) + (M.Low >> 1)) >> 63;
            A.High = N.High + M.High + C;
            A.Low = N.Low + M.Low;
        }


        private static void sub128(BigInteger N, BigInteger M, ref BigInteger A)
        {
            A.Low = N.Low - M.Low;
            ulong C = (((A.Low & M.Low) & 1) + (M.Low >> 1) + (A.Low >> 1)) >> 63;
            A.High = N.High - (M.High + C);
        }


        private static void shiftleft128(BigInteger N, uint S, ref BigInteger A)
        {
            ulong M1, M2;
            S &= 127;

            M1 = ((((S + 127) | S) & 64) >> 6) - 1UL;
            M2 = (S >> 6) - 1UL;
            S &= 63;
            A.High = (N.Low << (int)S) & (~M2);
            A.Low = (N.Low << (int)S) & M2;
            A.High |= ((N.High << (int)S) | ((N.Low >> (64 - (int)S)) & M1)) & M2;

            /*
                S &= 127;

                if(S != 0)
                {
                    if(S > 64)
                    {
                        A.High = N.Low << (S - 64);
                        A.Low = 0;
                    }
                    else if(S < 64)
                    {
                        A.High = (N.High << S) | (N.Low >> (64 - S));
                        A.Low = N.Low << S;
                    }
                    else
                    {
                        A.High = N.Low;
                        A.Low = 0;
                    }
                }
                else
                {
                    A.High = N.High;
                    A.Low = N.Low;
                }
                //*/
        }


        // https://en.wikipedia.org/wiki/C_data_types
        // llu unsigned long long (int) = ULONG
        // http://www.keil.com/forum/16968/default-type-when-only-unsigned-is-stated/
        // unsigned: In general, 'C' assumes that everything is an int unless specifically stated otherwise.
        // typedef unsigned long size_t;
        // The actual type of size_t is platform-dependent; a common mistake is to assume size_t is the same as unsigned int, which can lead to programming errors,2 particularly as 64-bit architectures become more prevalent.
        private static void shiftright128(BigInteger N, uint S, ref BigInteger A)
        {
            ulong M1, M2;
            S &= 127;

            M1 = ((((S + 127) | S) & 64) >> 6) - 1UL;
            M2 = (S >> 6) - 1UL;
            S &= 63;
            A.Low = (N.High >> (int)S) & (~M2);
            A.High = (N.High >> (int)S) & M2;
            A.Low |= ((N.Low >> (int)S) | ((N.High << (64 - (int)S)) & M1)) & M2;

            /*
            S &= 127;

            if(S != 0)
            {
                if(S > 64)
                {
                    A.High = N.High >> (S - 64);
                    A.Low = 0;
                }
                else if(S < 64)
                {
                    A.Low = (N.Low >> S) | (N.High << (64 - S));
                    A.High = N.High >> S;
                }
                else
                {
                    A.Low = N.High;
                    A.High = 0;
                }
            }
            else
            {
                A.High = N.High;
                A.Low = N.Low;
            }
            //*/
        }


        private static void not128(BigInteger N, ref BigInteger A)
        {
            A.High = ~N.High;
            A.Low = ~N.Low;
        }


        private static void or128(BigInteger N1, BigInteger N2, ref BigInteger A)
        {
            A.High = N1.High | N2.High;
            A.Low = N1.Low | N2.Low;
        }


        private static void and128(BigInteger N1, BigInteger N2, ref BigInteger A)
        {
            A.High = N1.High & N2.High;
            A.Low = N1.Low & N2.Low;
        }


        private static void xor128(BigInteger N1, BigInteger N2, ref BigInteger A)
        {
            A.High = N1.High ^ N2.High;
            A.Low = N1.Low ^ N2.Low;
        }



        private static ulong nlz128(BigInteger N)
        {
            return (N.High == 0) ? nlz64(N.Low) + 64 : nlz64(N.High);
        }


        private static ulong nlz64(ulong N)
        {
            ulong I;
            ulong C;

            I = ~N;
            C = ((I ^ (I + 1)) & I) >> 63;

            I = (N >> 32) + 0xffffffff;
            I = ((I & 0x100000000) ^ 0x100000000) >> 27;
            C += I; N <<= (int)I;

            I = (N >> 48) + 0xffff;
            I = ((I & 0x10000) ^ 0x10000) >> 12;
            C += I; N <<= (int)I;

            I = (N >> 56) + 0xff;
            I = ((I & 0x100) ^ 0x100) >> 5;
            C += I; N <<= (int)I;

            I = (N >> 60) + 0xf;
            I = ((I & 0x10) ^ 0x10) >> 2;
            C += I; N <<= (int)I;

            I = (N >> 62) + 3;
            I = ((I & 4) ^ 4) >> 1;
            C += I; N <<= (int)I;

            C += (N >> 63) ^ 1;

            return C;
        }


        private static ulong ntz128(BigInteger N)
        {
            return (N.Low == 0) ? ntz64(N.High) + 64 : ntz64(N.Low);
        }


        private static ulong ntz64(ulong N)
        {
            ulong I = ~N;
            ulong C = ((I ^ (I + 1)) & I) >> 63;

            I = (N & 0xffffffff) + 0xffffffff;
            I = ((I & 0x100000000) ^ 0x100000000) >> 27;
            C += I; N >>= (int)I;

            I = (N & 0xffff) + 0xffff;
            I = ((I & 0x10000) ^ 0x10000) >> 12;
            C += I; N >>= (int)I;

            I = (N & 0xff) + 0xff;
            I = ((I & 0x100) ^ 0x100) >> 5;
            C += I; N >>= (int)I;

            I = (N & 0xf) + 0xf;
            I = ((I & 0x10) ^ 0x10) >> 2;
            C += I; N >>= (int)I;

            I = (N & 3) + 3;
            I = ((I & 4) ^ 4) >> 1;
            C += I; N >>= (int)I;

            C += ((N & 1) ^ 1);

            return C;
        }


        private static ulong popcnt128(BigInteger N)
        {
            return popcnt64(N.High) + popcnt64(N.Low);
        }


        private static ulong popcnt64(ulong V)
        {
            // http://graphics.stanford.edu/~seander/bithacks.html#CountBitsSetParallel
            V -= ((V >> 1) & 0x5555555555555555);
            V = (V & 0x3333333333333333) + ((V >> 2) & 0x3333333333333333);
            return ((V + (V >> 4) & 0xF0F0F0F0F0F0F0F) * 0x101010101010101) >> 56;
        }


        public static int compare128(BigInteger N1, BigInteger N2)
        {
            return (((N1.High > N2.High) || ((N1.High == N2.High) && (N1.Low > N2.Low))) ? 1 : 0)
                 - (((N1.High < N2.High) || ((N1.High == N2.High) && (N1.Low < N2.Low))) ? 1 : 0);
        }


        // End Of Bitwise


        // MultSqr
        private static void mult64to128(ulong u, ulong v, ref ulong h, ref ulong l)
        {
            ulong u1 = (u & 0xffffffff);
            ulong v1 = (v & 0xffffffff);
            ulong t = (u1 * v1);
            ulong w3 = (t & 0xffffffff);
            ulong k = (t >> 32);

            u >>= 32;
            t = (u * v1) + k;
            k = (t & 0xffffffff);
            ulong w1 = (t >> 32);

            v >>= 32;
            t = (u1 * v) + k;
            k = (t >> 32);

            h = (u * v) + w1 + k;
            l = (t << 32) + w3;
        }


        private static void mult128(BigInteger N, BigInteger M, ref BigInteger Ans)
        {
            //PRINTVAR(N.High);
            //PRINTVAR(N.Low);
            //PRINTVAR(M.High);
            //PRINTVAR(M.Low);
            mult64to128(N.Low, M.Low, ref Ans.High, ref Ans.Low);
            //PRINTVAR(Ans.High);
            //PRINTVAR(Ans.Low);
            Ans.High += (N.High * M.Low) + (N.Low * M.High);
        }


        private static void mult128to256(BigInteger N, BigInteger M, ref BigInteger H, ref BigInteger L)
        {
            mult64to128(N.High, M.High, ref H.High, ref H.Low);
            mult64to128(N.Low, M.Low, ref L.High, ref L.Low);

            BigInteger T = new BigInteger();
            mult64to128(N.High, M.Low, ref T.High, ref T.Low);
            L.High += T.Low;
            if (L.High < T.Low)  // if L.High overflowed
            {
                inc128(H, ref H);
            }
            H.Low += T.High;
            if (H.Low < T.High)  // if H.Low overflowed
            {
                ++H.High;
            }

            mult64to128(N.Low, M.High, ref T.High, ref T.Low);
            L.High += T.Low;
            if (L.High < T.Low)  // if L.High overflowed
            {
                inc128(H, ref H);
            }
            H.Low += T.High;
            if (H.Low < T.High)  // if H.Low overflowed
            {
                ++H.High;
            }
        }


        private static void sqr64to128(ulong r, ref ulong h, ref ulong l)
        {
            ulong r1 = (r & 0xffffffff);
            ulong t = (r1 * r1);
            ulong w3 = (t & 0xffffffff);
            ulong k = (t >> 32);

            r >>= 32;
            ulong m = (r * r1);
            t = m + k;
            ulong w2 = (t & 0xffffffff);
            ulong w1 = (t >> 32);

            t = m + w2;
            k = (t >> 32);
            h = (r * r) + w1 + k;
            l = (t << 32) + w3;
        }


        private static void sqr128(BigInteger R, ref BigInteger Ans)
        {
            sqr64to128(R.Low, ref Ans.High, ref Ans.Low);
            Ans.High += (R.High * R.Low) << 1;
        }


        private static void sqr128to256(BigInteger R, ref BigInteger H, ref BigInteger L)
        {
            sqr64to128(R.High, ref H.High, ref H.Low);
            sqr64to128(R.Low, ref L.High, ref L.Low);

            BigInteger T = new BigInteger();
            mult64to128(R.High, R.Low, ref T.High, ref T.Low);

            H.High += (T.High >> 63);
            T.High = (T.High << 1) | (T.Low >> 63);  // Shift Left 1 bit
            T.Low <<= 1;

            L.High += T.Low;
            if (L.High < T.Low)  // if L.High overflowed
            {
                inc128(H, ref H);
            }

            H.Low += T.High;
            if (H.Low < T.High)  // if H.Low overflowed
            {
                ++H.High;
            }
        }



        // divmod 


        private static void div128(BigInteger M, BigInteger N, ref BigInteger Q)
        {
            BigInteger R = new BigInteger();
            divmod128(M, N, ref Q, ref R);
        }


        private static void mod128(BigInteger M, BigInteger N, ref BigInteger R)
        {
            BigInteger Q = new BigInteger();
            divmod128(M, N, ref Q, ref R);
        }


        private static void divmod128(BigInteger M, BigInteger N, ref BigInteger Q, ref BigInteger R)
        {
            ulong Nlz, Mlz, Ntz;
            int C;

            Nlz = nlz128(N);
            Mlz = nlz128(M);
            Ntz = ntz128(N);

            if (Nlz == 128)
            {
                throw new System.Exception("0");
            }
            else if ((M.High | N.High) == 0)
            {
                Q.High = R.High = 0;
                Q.Low = M.Low / N.Low;
                R.Low = M.Low % N.Low;
                return;
            }
            else if (Nlz == 127)
            {
                Q = M;
                R.High = R.Low = 0;
                return;
            }
            else if ((Ntz + Nlz) == 127)
            {
                shiftright128(M, (uint)Ntz, ref Q);
                dec128(N, ref N);
                and128(N, M, ref R);
                return;
            }

            C = compare128(M, N);
            if (C < 0)
            {
                Q.High = Q.Low = 0;
                R = M;
                return;
            }
            else if (C == 0)
            {
                Q.High = R.High = R.Low = 0;
                Q.Low = 1;
                return;
            }

            if ((Nlz - Mlz) > 5)
            {
                divmod128by128(M, N, ref Q, ref R);
            }
            else
            {
                bindivmod128(M, N, ref Q, ref R);
            }
        }


        private static void divmod128by128(BigInteger M, BigInteger N, ref BigInteger Q, ref BigInteger R)
        {
            if (N.High == 0)
            {
                if (M.High < N.Low)
                {
                    divmod128by64(M.High, M.Low, N.Low, ref Q.Low, ref R.Low);
                    Q.High = 0;
                    R.High = 0;
                    return;
                }
                else
                {
                    Q.High = M.High / N.Low;
                    R.High = M.High % N.Low;
                    divmod128by64(R.High, M.Low, N.Low, ref Q.Low, ref R.Low);
                    R.High = 0;
                    return;
                }
            }
            else
            {
                ulong n = nlz64(N.High);

                BigInteger v1 = new BigInteger();
                shiftleft128(N, (uint)n, ref v1);

                BigInteger u1 = new BigInteger();
                shiftright128(M, 1, ref u1);

                BigInteger q1 = new BigInteger();
                div128by64(u1.High, u1.Low, v1.High, ref q1.Low);
                q1.High = 0;
                shiftright128(q1, (uint)(63 - n), ref q1);

                if ((q1.High | q1.Low) != 0)
                {
                    dec128(q1, ref q1);
                }

                Q.High = q1.High;
                Q.Low = q1.Low;
                mult128(q1, N, ref q1);
                sub128(M, q1, ref R);

                if (compare128(R, N) >= 0)
                {
                    inc128(Q, ref Q);
                    sub128(R, N, ref R);
                }

                return;
            }
        }


        private static void div128by64(ulong u1, ulong u0, ulong v, ref ulong q)
        {
            const ulong b = 1UL << 32;
            ulong un1, un0, vn1, vn0, q1, q0, un32, un21, un10, rhat, vs, left, right;
            ulong s;

            s = nlz64(v);
            vs = v << (int)s;
            vn1 = vs >> 32;
            vn0 = vs & 0xffffffff;


            if (s > 0)
            {
                un32 = (u1 << (int)s) | (u0 >> (64 - (int)s));
                un10 = u0 << (int)s;
            }
            else
            {
                un32 = u1;
                un10 = u0;
            }


            un1 = un10 >> 32;
            un0 = un10 & 0xffffffff;

            q1 = un32 / vn1;
            rhat = un32 % vn1;

            left = q1 * vn0;
            right = (rhat << 32) | un1;

            again1:
            if ((q1 >= b) || (left > right))
            {
                --q1;
                rhat += vn1;
                if (rhat < b)
                {
                    left -= vn0;
                    right = (rhat << 32) | un1;
                    goto again1;
                }
            }

            un21 = (un32 << 32) + (un1 - (q1 * vs));

            q0 = un21 / vn1;
            rhat = un21 % vn1;

            left = q0 * vn0;
            right = (rhat << 32) | un0;
            again2:
            if ((q0 >= b) || (left > right))
            {
                --q0;
                rhat += vn1;
                if (rhat < b)
                {
                    left -= vn0;
                    right = (rhat << 32) | un0;
                    goto again2;
                }
            }

            q = (q1 << 32) | q0;
        }


        private static void divmod128by64(ulong u1, ulong u0, ulong v, ref ulong q, ref ulong r)
        {
            const ulong b = 1UL << 32;
            ulong un1, un0, vn1, vn0, q1, q0, un32, un21, un10, rhat, left, right;
            ulong s;

            s = nlz64(v);
            v <<= (int)s;
            vn1 = v >> 32;
            vn0 = v & 0xffffffff;

            if (s > 0)
            {
                un32 = (u1 << (int)s) | (u0 >> (64 - (int)s));
                un10 = u0 << (int)s;
            }
            else
            {
                un32 = u1;
                un10 = u0;
            }

            un1 = un10 >> 32;
            un0 = un10 & 0xffffffff;

            q1 = un32 / vn1;
            rhat = un32 % vn1;

            left = q1 * vn0;
            right = (rhat << 32) + un1;
            again1:
            if ((q1 >= b) || (left > right))
            {
                --q1;
                rhat += vn1;
                if (rhat < b)
                {
                    left -= vn0;
                    right = (rhat << 32) | un1;
                    goto again1;
                }
            }

            un21 = (un32 << 32) + (un1 - (q1 * v));

            q0 = un21 / vn1;
            rhat = un21 % vn1;

            left = q0 * vn0;
            right = (rhat << 32) | un0;
            again2:
            if ((q0 >= b) || (left > right))
            {
                --q0;
                rhat += vn1;
                if (rhat < b)
                {
                    left -= vn0;
                    right = (rhat << 32) | un0;
                    goto again2;
                }
            }

            r = ((un21 << 32) + (un0 - (q0 * v))) >> (int)s;
            q = (q1 << 32) | q0;
        }


        private static void bindivmod128(BigInteger M, BigInteger N, ref BigInteger Q, ref BigInteger R)
        {
            Q.High = Q.Low = 0;
            ulong Shift = nlz128(N) - nlz128(M);
            shiftleft128(N, (uint)Shift, ref N);

            do
            {
                shiftleft128(Q, 1, ref Q);
                if (compare128(M, N) >= 0)
                {
                    sub128(M, N, ref M);
                    Q.Low |= 1;
                }

                shiftright128(N, 1, ref N);
            } while (Shift-- != 0);

            R = M;
        }


    }


}
