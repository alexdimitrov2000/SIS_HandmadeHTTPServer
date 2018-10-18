namespace SIS.Framework.Attributes.Property
{
    using System.ComponentModel.DataAnnotations;

    public class NumberRangeAttribute : ValidationAttribute
    {
        private readonly double minimumValue;
        private readonly double maximumValue;

        public NumberRangeAttribute(double minValue = double.MinValue, double maxValue = double.MaxValue)
        {
            this.minimumValue = minValue;
            this.maximumValue = maxValue;
        }

        public override bool IsValid(object value)
        {
            return (double) value >= this.minimumValue && (double) value <= this.maximumValue;
        }
    }
}
