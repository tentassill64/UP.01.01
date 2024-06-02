using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSUniversalLib
{
    public class Calculation
    {
        private const decimal ProductType1Coefficient = 1.1m;
        private const decimal ProductType2Coefficient = 2.5m;
        private const decimal ProductType3Coefficient = 8.43m;

        private const decimal MaterialType1BrakePercentage = 0.3m / 100;
        private const decimal MaterialType2BrakePercentage = 0.12m / 100;

        public static int GetQuantityForProduct(int productType, int materialType, int count, float width, float length)
        {
            if (productType < 1 || productType > 3 || materialType < 1 || materialType > 2 || count <= 0 || width <= 0 || length <= 0)
            {
                return -1;
            }

            decimal productArea = (decimal)width * (decimal)length;
            decimal productTypeCoefficient;

            if (productType == 1)
            {
                productTypeCoefficient = ProductType1Coefficient;
            }
            else if (productType == 2)
            {
                productTypeCoefficient = ProductType2Coefficient;
            }
            else
            {
                productTypeCoefficient = ProductType3Coefficient;
            }

            decimal requiredQualityMaterial = productArea * productTypeCoefficient * count;
            decimal materialTypeBrakePercentage;

            if (materialType == 1)
            {
                materialTypeBrakePercentage = MaterialType1BrakePercentage;
            }
            else
            {
                materialTypeBrakePercentage = MaterialType2BrakePercentage;
            }

            decimal totalRequiredMaterial = requiredQualityMaterial / (1 - materialTypeBrakePercentage);
            return (int)Math.Ceiling(totalRequiredMaterial);
        }
    }


}
