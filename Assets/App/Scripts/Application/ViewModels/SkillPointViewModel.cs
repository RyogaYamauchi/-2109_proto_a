namespace App.Application
{
    public readonly struct SkillPointViewModel
    {
        public readonly int MaxValue;
        public readonly int Value;

        public SkillPointViewModel(int value, int maxValue)
        {
            Value = value;
            MaxValue = maxValue;
        }
    }
}