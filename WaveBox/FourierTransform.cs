using System;

namespace BoonieBear.DeckUnit.WaveBox
{
    public class FourierTransform
    {
        static private int n, nu;

        static private int BitReverse(int j)
        {
            int j2;
            int j1 = j;
            int k = 0;
            for (int i = 1; i <= nu; i++)
            {
                j2 = j1 / 2;
                k = 2 * k + j1 - 2 * j2;
                j1 = j2;
            }
            return k;
        }

        static public double[] FFT(ref double[] x)
        {
            
            n = x.Length;
            
            nu = (int)(Math.Log(n) / Math.Log(2));
            if (Math.Pow(2, nu) < n) //n is not a power of 2
            {
                nu++;
                n = (int)Math.Pow(2, nu);
            }
            int n2 = n / 2;
            int nu1 = nu - 1;
            double[] xre = new double[n];
            double[] xim = new double[n];
            if (n < 128)//too short
                return null;
            double[] magnitude = new double[n2];
            
            double[] decibel = new double[n2];
            
            double tr, ti, p, arg, c, s;
            for (int i = 0; i < n; i++)
            {
                if (i < x.Length)
                    xre[i] = x[i];
                else
                {
                    xre[i] = 0;
                }
                xim[i] = 0.0f;
            }
            int k = 0;
            for (int l = 1; l <= nu; l++)
            {
                while (k < n)
                {
                    for (int i = 1; i <= n2; i++)
                    {
                        p = BitReverse(k >> nu1);
                        arg = 2 * (double)Math.PI * p / n;
                        c = (double)Math.Cos(arg);
                        s = (double)Math.Sin(arg);
                        tr = xre[k + n2] * c + xim[k + n2] * s;
                        ti = xim[k + n2] * c - xre[k + n2] * s;
                        xre[k + n2] = xre[k] - tr;
                        xim[k + n2] = xim[k] - ti;
                        xre[k] += tr;
                        xim[k] += ti;
                        k++;
                    }
                    k += n2;
                }
                k = 0;
                nu1--;
                n2 = n2 / 2;
            }
            k = 0;
            int r;
            while (k < n)
            {
                r = BitReverse(k);
                if (r > k)
                {
                    tr = xre[k];
                    ti = xim[k];
                    xre[k] = xre[r];
                    xim[k] = xim[r];
                    xre[r] = tr;
                    xim[r] = ti;
                }
                k++;
            }
            for (int i = 0; i < n / 2; i++)
            {
                decibel[i] = 10.0 * Math.Log10((float)(Math.Sqrt((xre[i] * xre[i]) + (xim[i] * xim[i]))));
                if (double.IsNegativeInfinity(decibel[i]))
                    decibel[i] = 0;
            }
            return decibel;
        }
    }
}
