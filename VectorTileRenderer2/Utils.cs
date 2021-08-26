
namespace VectorTileRenderer
{


    internal static class Utils
    {


        public static double ConvertRange(double oldValue, double oldMin, double oldMax, double newMin, double newMax, bool clamp = false)
        {
            double NewRange;
            double NewValue;
            double OldRange = (oldMax - oldMin);

            if (OldRange == 0)
            {
                NewValue = newMin;
            }
            else
            {
                NewRange = (newMax - newMin);
                NewValue = (((oldValue - oldMin) * NewRange) / OldRange) + newMin;
            }

            if (clamp)
            {
                NewValue = System.Math.Min(System.Math.Max(NewValue, newMin), newMax);
            }

            return NewValue;
        } // End Function ConvertRange 


        public static string Sha256(string randomString)
        {
            System.Text.StringBuilder hash = new System.Text.StringBuilder();

            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] crypto = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(randomString));

                foreach (byte theByte in crypto)
                {
                    hash.Append(theByte.ToString("x2"));
                } // Next theByte 

            } // End Using sha256 

            return hash.ToString();
        } // End Function Sha256 


    } // End Module Utils 


} // End Namespace VectorTileRenderer 
