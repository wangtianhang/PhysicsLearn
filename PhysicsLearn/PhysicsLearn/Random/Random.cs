using UnityEngine;

namespace Common
{
    /// <summary>
    /// 禁止非战斗逻辑模块使用此随机数
    /// </summary>
    public static class Random
    {
        static UnityRandom m_unityRandom;

        public static void Test()
        {
            for (int i = 0; i < 100; ++i)
            {
                float random = Range(0f, 1f);
                Debug.Log("i " + i + " Random " + random);
            }
            for(int i = 0; i < 100; ++i)
            {
                Debug.Log(Random.Range(0, 2));
            }
        }

        ///static System.Random s_random = null;
        /*static long s_curRandom = 0;
        static int s_seed = 0;
        public static int s_index = 0;

        public static void SetSeed(int seed)
        {
            s_seed = seed;
            Debug.Log("SetSeed " + seed);
            //s_random = new System.Random(s_seed);
            s_curRandom = s_seed;
        }

        /// <summary>
        /// 左闭右开区间 与unity一致
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static int Range(int minInclusive, int maxExclusive)
        {
            s_index++;
//             if (s_random == null)
//             {
//                 s_random = new System.Random(s_seed);
//             }

            int ret = Next(minInclusive, maxExclusive);
            //Debug.Log("Range int Index " + s_index + " " + ret);
            return ret;
        }

        /// <summary>
        /// 左闭右闭区间 与unity一致
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public static float Range(float minInclusive, float maxInclusive)
        {
            s_index++;
//             if (s_random == null)
//             {
//                 s_random = new System.Random(s_seed);
//             }

            int randomInteger = Next(0, 1000);
            float randomFloat = (float)randomInteger / (float)1000;
            float range = maxInclusive - minInclusive;
            float ret = minInclusive + randomFloat * range;
            //Debug.Log("Range float Index " + s_index + " " + ret);
            return ret;
        }

        static int Next(int minInclusive, int maxExclusive)
        {
            int num = maxExclusive - minInclusive;
//             int ret = (int)(Sample() * num) + minInclusive;
//             return ret;
            int last = Rand() % num;
            int ret = minInclusive + last;
            return ret;
        }

//         static double Sample()
//         {
//             double sample = Rand() / int.MaxValue;
//             return sample;
//         }

        static int Rand()
        {
            s_curRandom = (s_curRandom * 214013L + 2531011L) >> 16 & 0x7fff;
            return (int)s_curRandom;

            //s_curRandom = s_curRandom * 1103515245 + 12345;
            //return (int)((s_curRandom / 65536) % 32768);
        }*/

        public static void Reset(int seed = 0)
        {
            m_unityRandom = new UnityRandom(seed);
        }

        public static int Range(int minInclusive, int maxExclusive)
        {
            if(m_unityRandom == null)
            {
                m_unityRandom = new UnityRandom();
            }
            if(minInclusive == maxExclusive - 1)
            {
                return minInclusive;
            }
            float ret = m_unityRandom.Range(minInclusive, maxExclusive - 1);
            return (int)ret;
        }

        public static float Range(float minInclusive, float maxInclusive)
        {
            if (m_unityRandom == null)
            {
                m_unityRandom = new UnityRandom();
            }
            return minInclusive + m_unityRandom.Value() * (maxInclusive - minInclusive);
        }
    }
}
