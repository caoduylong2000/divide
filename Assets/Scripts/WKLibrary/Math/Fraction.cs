using System;

using UnityEngine;//for Debug Print

namespace WK { namespace Math {

    public class Fraction : IComparable< Fraction >
    {
        private int numerator;//分子
        private int denominator;//分母

        //------------------------------------------------------------------------------
        public int Numerator {
            get { return numerator; }
        }

        //------------------------------------------------------------------------------
        public int Denominator {
            get { return denominator; }
        }


        //------------------------------------------------------------------------------
        public float Decimal {
            get { return (float)numerator / (float)denominator; }
        }

        //------------------------------------------------------------------------------
        public Fraction( int n, int d )
        {
            numerator = n;
            denominator = d;
            arrangeMinus();
            reduce();
        }

        //------------------------------------------------------------------------------
        public Fraction( int n )
        {
            numerator = n;
            denominator = 1;
            arrangeMinus();
        }

        //------------------------------------------------------------------------------
        private void reduce()
        {
            //@memo 0/0 remains as 0/0
            if( denominator == 0 ) { return ; } // if denominator equals 0, then do nothing.
            if( numerator == 0 ) { denominator = 1; return ; }

            int divisor = GetGreatestCommonDivisor( numerator, denominator );

            try
            {
                numerator /= divisor;
                denominator /= divisor;
            }
            catch( ArithmeticException e )
            {
                Debug.Log( string.Format( "{0}, {1}, {2}", divisor, numerator, denominator ) );
                throw e;
            }
        }

        //マイナスの掛かるのは分子のみにする
        //------------------------------------------------------------------------------
        private void arrangeMinus()
        {
            if( denominator < 0 )
            {
                denominator *= -1;
                numerator   *= -1;
            }
        }

        //------------------------------------------------------------------------------
        public static int GetGreatestCommonDivisor( int a, int b )
        {
            try {
                a = System.Math.Abs( a );
                b = System.Math.Abs( b );

                if( a > b )
                {
                    WK.Utils.Utils.Swap< int >( ref a, ref b );
                }

                if( a == 0 )//0が含まれてたら0を返す
                {
                    return 0;
                }

                while( b!= 0 )
                {
                    int r = a % b;
                    a = b;
                    b = r;
                }

                if( a == 0 )
                {
                    Debug.Log( a.ToString() + "," + b.ToString() );
                    Debug.Assert( false );
                }
                return a;
            }
            catch( ArithmeticException e )
            {
                Debug.Log( string.Format( "{0}, {1}", a, b ) );
                throw e;
            }
        } 
        
        //------------------------------------------------------------------------------
        public static Fraction operator + ( Fraction a, Fraction b )
        {
            int denominator = a.Denominator * b.Denominator;
            int numerator = a.Numerator * b.Denominator + a.Denominator * b.Numerator;
            return new Fraction( numerator, denominator );
        }
        
        //------------------------------------------------------------------------------
        public static Fraction operator + ( Fraction a, int b )
        {
            return a + new Fraction(b);
        }

        //------------------------------------------------------------------------------
        public static Fraction operator - ( Fraction a, Fraction b )
        {
            return a + ( b * -1 );
        }
        
        //------------------------------------------------------------------------------
        public static Fraction operator - ( Fraction a, int b )
        {
            return a - new Fraction( b );
        }

        //------------------------------------------------------------------------------
        public static Fraction operator * ( Fraction a, Fraction b )
        {
            return new Fraction( a.Numerator * b.Numerator, a.Denominator * b.Denominator );
        }

        //------------------------------------------------------------------------------
        public static Fraction operator * ( Fraction a, int b )
        {
            return a * new Fraction( b );
        }

        //------------------------------------------------------------------------------
        public static Fraction operator / ( Fraction a, Fraction b )
        {
            /* if( b.numerator == 0 ) */
            /* { */
            /*     throw new ArgumentException( "Error!: zero dividing !!!" ); */
            /* } */
            return new Fraction( a.Numerator * b.Denominator, a.Denominator * b.Numerator );
        }
        
        //------------------------------------------------------------------------------
        public static Fraction operator / ( Fraction a, int b )
        {
            return a / new Fraction( b );
        }
        
        //------------------------------------------------------------------------------
        public static bool operator == ( Fraction a, Fraction b )
        {
            //confirm whether obj is null or not. 
            //you have to convert to "object" type.
            //If you don't do so, it this code becomes infinite loop.
            if ( (object)a == null )
            {
                return ( (object)b == null );
            }
            if ( (object)b == null )
            {
                return false;
            }

            return ( a.Numerator == b.Numerator ) && ( a.Denominator == b.Denominator );
        }

        //------------------------------------------------------------------------------
        public static bool operator == ( Fraction a, int b )
        {
            return ( a == new Fraction( b ) );
        }

        //------------------------------------------------------------------------------
        public static bool operator != ( Fraction a, Fraction b )
        {
            return !( a == b );
        }

        //------------------------------------------------------------------------------
        public static bool operator != ( Fraction a, int b )
        {
            return ( a != new Fraction( b ) );
        }

        //------------------------------------------------------------------------------
        public override bool Equals( object obj )
        {
            if ( ( object )obj == null || this.GetType() != obj.GetType())
            {
                return false;
            }
            Fraction f = ( Fraction )obj;
            return ( this.numerator == f.numerator ) && ( this.denominator == f.denominator );
        }

        //------------------------------------------------------------------------------
        public override int GetHashCode()
        {
            return numerator ^ denominator.GetHashCode();
        }

        //------------------------------------------------------------------------------
        public int CompareTo( Fraction other )
        {
            if ( object.ReferenceEquals( other, null ) )
            {
                return 1;// All instances are greater than null
            }

            int l = numerator * other.Denominator;
            int r = other.Numerator * denominator;

            return l - r;
        }

        //------------------------------------------------------------------------------
        public bool IsZeroDivide()
        {
            return denominator == 0;
        }

        //------------------------------------------------------------------------------
        public override string ToString()
        {
            if( denominator == 1 ) return numerator.ToString();
            return string.Format( "{0}/{1}", numerator, denominator );
        }

    }

}
}
