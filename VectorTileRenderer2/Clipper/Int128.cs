namespace ClipperLib
{
	internal struct Int128
	{
		private long hi;

		private ulong lo;

		public Int128(long _lo)
		{
			lo = (ulong)_lo;
			if (_lo < 0)
			{
				hi = -1L;
			}
			else
			{
				hi = 0L;
			}
		}

		public Int128(long _hi, ulong _lo)
		{
			lo = _lo;
			hi = _hi;
		}

		public Int128(Int128 val)
		{
			hi = val.hi;
			lo = val.lo;
		}

		public bool IsNegative()
		{
			return hi < 0;
		}

		public static bool operator ==(Int128 val1, Int128 val2)
		{
			if ((object)val1 == (object)val2)
			{
				return true;
			}
			if ((object)val1 == null || (object)val2 == null)
			{
				return false;
			}
			if (val1.hi == val2.hi)
			{
				return val1.lo == val2.lo;
			}
			return false;
		}

		public static bool operator !=(Int128 val1, Int128 val2)
		{
			return !(val1 == val2);
		}

		public override bool Equals(object obj)
		{
			if (obj == null || !(obj is Int128))
			{
				return false;
			}
			Int128 @int = (Int128)obj;
			if (@int.hi == hi)
			{
				return @int.lo == lo;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return hi.GetHashCode() ^ lo.GetHashCode();
		}

		public static bool operator >(Int128 val1, Int128 val2)
		{
			if (val1.hi != val2.hi)
			{
				return val1.hi > val2.hi;
			}
			return val1.lo > val2.lo;
		}

		public static bool operator <(Int128 val1, Int128 val2)
		{
			if (val1.hi != val2.hi)
			{
				return val1.hi < val2.hi;
			}
			return val1.lo < val2.lo;
		}

		public static Int128 operator +(Int128 lhs, Int128 rhs)
		{
			lhs.hi += rhs.hi;
			lhs.lo += rhs.lo;
			if (lhs.lo < rhs.lo)
			{
				lhs.hi++;
			}
			return lhs;
		}

		public static Int128 operator -(Int128 lhs, Int128 rhs)
		{
			return lhs + -rhs;
		}

		public static Int128 operator -(Int128 val)
		{
			if (val.lo == 0)
			{
				return new Int128(-val.hi, 0uL);
			}
			return new Int128(~val.hi, ~val.lo + 1);
		}

		public static Int128 Int128Mul(long lhs, long rhs)
		{
			bool flag = lhs < 0 != rhs < 0;
			if (lhs < 0)
			{
				lhs = -lhs;
			}
			if (rhs < 0)
			{
				rhs = -rhs;
			}
			ulong num = (ulong)lhs >> 32;
			ulong num2 = (ulong)(lhs & 0xFFFFFFFFu);
			ulong num3 = (ulong)rhs >> 32;
			ulong num4 = (ulong)(rhs & 0xFFFFFFFFu);
			ulong num5 = num * num3;
			ulong num6 = num2 * num4;
			ulong num7 = num * num4 + num2 * num3;
			long num8 = (long)(num5 + (num7 >> 32));
			ulong num9 = (num7 << 32) + num6;
			if (num9 < num6)
			{
				num8++;
			}
			Int128 @int = new Int128(num8, num9);
			if (!flag)
			{
				return @int;
			}
			return -@int;
		}

		public static Int128 operator /(Int128 lhs, Int128 rhs)
		{
			if (rhs.lo == 0 && rhs.hi == 0)
			{
				throw new ClipperException("Int128: divide by zero");
			}
			bool flag = rhs.hi < 0 != lhs.hi < 0;
			if (lhs.hi < 0)
			{
				lhs = -lhs;
			}
			if (rhs.hi < 0)
			{
				rhs = -rhs;
			}
			if (rhs < lhs)
			{
				Int128 @int = new Int128(0L);
				Int128 int2 = new Int128(1L);
				while (rhs.hi >= 0 && !(rhs > lhs))
				{
					rhs.hi <<= 1;
					if ((long)rhs.lo < 0L)
					{
						rhs.hi++;
					}
					rhs.lo <<= 1;
					int2.hi <<= 1;
					if ((long)int2.lo < 0L)
					{
						int2.hi++;
					}
					int2.lo <<= 1;
				}
				rhs.lo >>= 1;
				if ((rhs.hi & 1) == 1)
				{
					rhs.lo |= 9223372036854775808uL;
				}
				rhs.hi = (long)((ulong)rhs.hi >> 1);
				int2.lo >>= 1;
				if ((int2.hi & 1) == 1)
				{
					int2.lo |= 9223372036854775808uL;
				}
				int2.hi >>= 1;
				while (int2.hi != 0 || int2.lo != 0)
				{
					if (!(lhs < rhs))
					{
						lhs -= rhs;
						@int.hi |= int2.hi;
						@int.lo |= int2.lo;
					}
					rhs.lo >>= 1;
					if ((rhs.hi & 1) == 1)
					{
						rhs.lo |= 9223372036854775808uL;
					}
					rhs.hi >>= 1;
					int2.lo >>= 1;
					if ((int2.hi & 1) == 1)
					{
						int2.lo |= 9223372036854775808uL;
					}
					int2.hi >>= 1;
				}
				if (!flag)
				{
					return @int;
				}
				return -@int;
			}
			if (rhs == lhs)
			{
				return new Int128(1L);
			}
			return new Int128(0L);
		}

		public double ToDouble()
		{
			if (hi < 0)
			{
				if (lo == 0)
				{
					return (double)hi * 1.8446744073709552E+19;
				}
				return 0.0 - ((double)(~lo) + (double)(~hi) * 1.8446744073709552E+19);
			}
			return (double)lo + (double)hi * 1.8446744073709552E+19;
		}
	}
}
