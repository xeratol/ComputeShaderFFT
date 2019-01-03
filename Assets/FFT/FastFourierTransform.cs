using System;
using System.Collections.Generic;
using System.Numerics;

public class FastFourierTransform
{
    public static uint RoundUpPowerOf2(uint v)
    {
        uint ans = 1;
        var numOnes = 0u;
        while (v != 0)
        {
            if ((v & 1) != 0)
            {
                ++numOnes;
            }
            v >>= 1;
            ans <<= 1;
        }
        if (numOnes < 2)
        {
            ans >>= 1;
        }
        return ans;
    }

    public static List<uint> GenBitReversal(uint size)
    {
        List <uint> allReversed = new List<uint>();
        var numBits = (uint)Math.Log(size, 2.0);

        for (var i = 0u; i < size; ++i)
        {
            var orig = i;
            var reversed = 0u;
            for (var j = 0u; j < numBits; ++j)
            {
                reversed <<= 1;
                if ( (orig & 1) != 0)
                {
                    reversed |= 1;
                }
                orig >>= 1;
            }
            allReversed.Add(reversed);
        }

        return allReversed;
    }

    public static List<Complex> GenTwiddleFactors(uint halfN)
    {
        List<Complex> twiddle = new List<Complex>();
        for (var i = 0u; i < halfN; ++i)
        {
            twiddle.Add( Complex.FromPolarCoordinates(1.0, -i * Math.PI / halfN) );
        }
        return twiddle;
    }

    public static List<Complex> ApplyButterfly(List<Complex> data, List<Complex> twiddle)
    {
        var N = data.Count;
        List<Complex> retVal = new List<Complex>(data);
        for (var numElements = 2; numElements <= N; numElements *= 2)
        {
            for (var offset = 0; offset < N; offset += numElements)
            {
                List <Complex> temp = retVal.GetRange(offset, numElements);

                var halfNumElements = numElements / 2;
                for (var i = 0; i < halfNumElements; ++i)
                {
                    retVal[i + offset]                   = temp[i] + temp[i + halfNumElements] * twiddle[i * N / numElements];
                    retVal[i + offset + halfNumElements] = temp[i] - temp[i + halfNumElements] * twiddle[i * N / numElements];
                }
            }
        }
        return retVal;
    }

    public static List<Complex> Solve(List<Complex> data, List<uint> bitRev, List<Complex> twiddle)
    {
        var N = RoundUpPowerOf2((uint)data.Count);

        if (twiddle.Count * 2 != N)
        {
            throw new Exception("Invalid Twiddle Factors");
        }

        if (bitRev.Count != N)
        {
            throw new Exception("Invalid Bit Reversal Indices");
        }

        if (data.Count < N)
        {
            throw new Exception("data is not a power of 2");
        }

        List<Complex> d = new List<Complex>(data.Count);
        for (var i = 0; i<N; ++i)
        {
            d.Add( data[(int)bitRev[i]] );
        }

        return ApplyButterfly(d, twiddle);
    }
}
