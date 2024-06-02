using NUnit.Framework;
using WSUniversalLib;

namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void GetQuantityForProduct_ValidInputs_ReturnsExpectedResult()
        {
            int productType = 3;
            int materialType = 1;
            int count = 15;
            float width = 20;
            float length = 45;

            int expectedResult = 114148;
            int actualResult = Calculation.GetQuantityForProduct(productType, materialType, count, width, length);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetQuantityForProduct_NonExistentProductType_ReturnsMinusOne()
        {
            int productType = 4;
            int materialType = 1;
            int count = 15;
            float width = 20;
            float length = 45;

            int expectedResult = -1;
            int actualResult = Calculation.GetQuantityForProduct(productType, materialType, count, width, length);

            Assert.AreEqual(expectedResult, actualResult);
        }
        [Test]
        public void GetQuantityForProduct_NonExistentMaterialType_ReturnsMinusOne()
        {
            int productType = 2;
            int materialType = 3;
            int count = 15;
            float width = 20;
            float length = 45;

            int expectedResult = -1;
            int actualResult = Calculation.GetQuantityForProduct(productType, materialType, count, width, length);

            Assert.AreEqual(expectedResult, actualResult);
        }
        [Test]
        public void GetQuantityForProduct_ZeroCount_ReturnsMinusOne()
        {
            int productType = 2;
            int materialType = 2;
            int count = 0;
            float width = 20;
            float length = 45;

            int expectedResult = -1;
            int actualResult = Calculation.GetQuantityForProduct(productType, materialType, count, width, length);

            Assert.AreEqual(expectedResult, actualResult);
        }
        [Test]
        public void GetProductForProduct_ZeroWidth_ReturnsMinusOne()
        {
            int productType = 2;
            int materialType = 2;
            int count = 15;
            float width = 0;
            float length = 45;

            int expectedResult = -1;
            int actualResult = Calculation.GetQuantityForProduct(productType, materialType, count, width, length);

            Assert.AreEqual(expectedResult, actualResult);
        }
        [Test]
        public void GetProductForProduct_ZeroLength_ReturnsMinusOne()
        {
            int productType = 2;
            int materialType = 2;
            int count = 15;
            float width = 20;
            float length = 0;

            int expectedResult = -1;
            int actualResult = Calculation.GetQuantityForProduct(productType, materialType, count, width, length);

            Assert.AreEqual(expectedResult, actualResult);
        }
        [Test]
        public void GetQuantityForProduct_NegativeLength_ReturnsMinusOne()
        {
            int productType = 2;
            int materialType = 2;
            int count = 15;
            float width = -20;
            float length = 45;

            int expectedResult = -1;
            int actualResult = Calculation.GetQuantityForProduct(productType, materialType, count, width, length);

            Assert.AreEqual(expectedResult, actualResult);
        }
        [Test]
        public void GetQuantityForProduct_NegativeWidth_ReturnsMinusOne()
        {
            int productType = 2;
            int materialType = 2;
            int count = 15;
            float width = 20;
            float length = -45;

            int expectedResult = -1;
            int actualResult = Calculation.GetQuantityForProduct(productType, materialType, count, width, length);

            Assert.AreEqual(expectedResult, actualResult);
        }
        [Test]
        public void GetQuantityForProduct_LargeWidth_ReturnsExpectedResult()
        {
            int productType = 2;
            int materialType = 2;
            int count = 15;
            float width = 20;
            float length = 45000;

            int expectedResult = 33790549;
            int actualResult = Calculation.GetQuantityForProduct(productType, materialType, count, width, length);

            Assert.AreEqual(expectedResult, actualResult);
        }
        [Test]
        public void GetQuantityForProduct_LargeCount_ReturnsExpectedResult()
        {
            int productType = 2;
            int materialType = 2;
            int count = 99999;
            float width = 20;
            float length = 45;

            int expectedResult = 225268072;
            int actualResult = Calculation.GetQuantityForProduct(productType, materialType, count, width, length);

            Assert.AreEqual(expectedResult, actualResult);
        }
        [Test]
        public void GetQuantityForProduct_ProductTypeWithHighestCoefficient_ReturnsExpectedResult()
        {
            int productType = 3;
            int materialType = 1;
            int count = 15;
            float width = 20;
            float length = 45;

            int expectedResult = 114148;
            int actualResult = Calculation.GetQuantityForProduct(productType, materialType, count, width, length);

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetQuantityForProduct_ProductTypeWithLowestCoefficient_ReturnsExpectedResult()
        {
            int productType = 1;
            int materialType = 1;
            int count = 15;
            float width = 20;
            float length = 45;

            int expectedResult = 14895;
            int actualResult = Calculation.GetQuantityForProduct(productType, materialType, count, width, length);

            Assert.AreEqual(expectedResult, actualResult);
        }
        [Test]
        public void GetQuantityForProduct_MaterialTypeWithHighestBrakePercentage_ReturnsExpectedResult()
        {
            int productType = 3;
            int materialType = 1;
            int count = 15;
            float width = 20;
            float length = 45;

            int expectedResult = 114148;
            int actualResult = Calculation.GetQuantityForProduct(productType, materialType, count, width, length);

            Assert.AreEqual(expectedResult, actualResult);
        }
        [Test]
        public void GetQuantityForProduct_MaterialTypeWithLowestBrakePercentage_ReturnsExpectedResult()
        {
            int productType = 2;
            int materialType = 2;
            int count = 10;
            float width = 15;
            float length = 30;

            int expectedResult = 11264;
            int actualResult = Calculation.GetQuantityForProduct(productType, materialType, count, width, length);

            Assert.AreEqual(expectedResult, actualResult);
        }
        [Repeat(1000)]
        [Test]
        public void GetQuantityForProduct_StressTesting_ReturnsExpectedResult()
        {
                Random rand = new Random();
                int productType = rand.Next(1, 4);
                int materialType = rand.Next(1, 3);
                int count = rand.Next(1, 100);
                float width = rand.Next(1, 100) + (float)rand.NextDouble();
                float length = rand.Next(1, 100) + (float)rand.NextDouble();

                float productCoefficient = productType == 1 ? 1.1f : productType == 2 ? 2.5f : 8.43f;
                float materialBrake = materialType == 1 ? 0.003f : 0.0012f;
                int expectedResult = (int)Math.Round(productCoefficient * width * length * count * (1 + materialBrake)+1);

                int actualResult = Calculation.GetQuantityForProduct(productType, materialType, count, width, length);

                Assert.AreEqual(expectedResult, actualResult);
            
        }
    }
}
