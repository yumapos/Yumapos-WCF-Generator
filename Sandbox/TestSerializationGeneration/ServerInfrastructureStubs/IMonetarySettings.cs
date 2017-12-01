namespace TestSerializationGeneration
{
    public interface IMonetarySettings
    {
        bool PriceIncludesTax { get; }

        int Precision { get; }

        decimal Round(decimal toRound);

        decimal Floor(decimal toRound);

        PosMoney CalculateTaxAmount(PosMoney money, float taxRate);

        void Init();
    }
}