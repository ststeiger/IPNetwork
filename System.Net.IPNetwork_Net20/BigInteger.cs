using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System
{
    public class Lazy<T>
    {

        public T Value;


        public Lazy(System.Func<T> abc)
        { }

    }
}



namespace System.Numerics 
{
    
    public class DataContractAttribute : System.Attribute
    { }


    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DataMemberAttribute : Attribute
    {
        string name;
        bool isNameSetExplicitly;
        int order = -1;
        bool isRequired;
        bool emitDefaultValue = false; // Globals.DefaultEmitDefaultValue;

        public DataMemberAttribute()
        {
        }

        public string Name
        {
            get { return name; }
            set { name = value; isNameSetExplicitly = true; }
        }

        public bool IsNameSetExplicitly
        {
            get { return isNameSetExplicitly; }
        }

        public int Order
        {
            get { return order; }
            set
            {
                if (value < 0)
                    throw new System.NotSupportedException("Exception not supported");
                order = value;
            }
        }

        public bool IsRequired
        {
            get { return isRequired; }
            set { isRequired = value; }
        }

        public bool EmitDefaultValue
        {
            get { return emitDefaultValue; }
            set { emitDefaultValue = value; }
        }
    }


    public struct SmallInteger
       : IComparable<SmallInteger>
    //  IComparable, IComparable<SmallInteger>, IEquatable<SmallInteger>, IFormattable
    {


        int IComparable<SmallInteger>.CompareTo(SmallInteger other)
        {
            return 0;
        }


        public int CompareTo(SmallInteger other)
        {
            return ((IComparable<SmallInteger>)this).CompareTo(other);
        }


        public int Sign { get; set; }

        public SmallInteger(byte[] b)
        {
            this.Sign = 1;
        }

        public SmallInteger(uint b)
        {
            this.Sign = 1;
        }


        public static SmallInteger operator +(SmallInteger left, SmallInteger right)
        {
            return new SmallInteger();
        }


        public static SmallInteger operator +(SmallInteger left, int right)
        {
            return new SmallInteger();
        }

        public static SmallInteger operator -(SmallInteger left, SmallInteger right)
        {
            return new SmallInteger();
        }


        public static SmallInteger operator -(SmallInteger left, int right)
        {
            return new SmallInteger();
        }



        public static SmallInteger operator *(SmallInteger left, SmallInteger right)
        {
            return new SmallInteger();
        }

        public static SmallInteger operator /(SmallInteger left, SmallInteger right)
        {
            return new SmallInteger();
        }


        public static SmallInteger operator ++(SmallInteger value)
        {
            return new SmallInteger();
        }

        public static SmallInteger operator --(SmallInteger value)
        {
            return new SmallInteger();
        }



        public static bool operator <(SmallInteger left, SmallInteger right)
        {
            return false;
        }
        
        public static bool operator >(SmallInteger left, SmallInteger right)
        {
            return false;
        }

        public static bool operator <=(SmallInteger left, SmallInteger right)
        {
            return false;
        }

        public static bool operator >=(SmallInteger left, SmallInteger right)
        {
            return false;
        }

        public static bool operator <=(SmallInteger left, int right)
        {
            return false;
        }

        public static bool operator >=(SmallInteger left, int right)
        {
            return false;
        }

        public static bool operator ==(SmallInteger left, SmallInteger right)
        {
            return false;
        }

        public static bool operator !=(SmallInteger left, SmallInteger right)
        {
            return false;
        }


        public static bool operator ==(SmallInteger left, int right)
        {
            return false;
        }

        public static bool operator !=(SmallInteger left, int right)
        {
            return false;
        }



        public byte[] ToByteArray()
        {
            return null;
        }


        public string ToString(string format)
        {
            return null;
        }



        public static SmallInteger Pow(int a, int b)
        {
            return new SmallInteger();
        }

        public static SmallInteger operator ~(SmallInteger left)
        {
            return new SmallInteger();
        }

        public static SmallInteger operator &(SmallInteger left, SmallInteger right)
        {
            return new SmallInteger();
        }


        public static SmallInteger operator |(SmallInteger left, SmallInteger right)
        {
            return new SmallInteger();
        }


        public static SmallInteger operator ^(SmallInteger left, SmallInteger right)
        {
            return new SmallInteger();
        }




        public static SmallInteger operator <<(SmallInteger value, int shift)
        {
            return new SmallInteger();
        }

        public static SmallInteger operator >>(SmallInteger value, int shift)
        {
            return new SmallInteger();
        }



        public static implicit operator SmallInteger(int d)
        {
            return new SmallInteger();
        }

        public static implicit operator SmallInteger(uint d)
        {
            return new SmallInteger();
        }




    }
}
