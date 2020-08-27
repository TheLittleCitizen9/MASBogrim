namespace MASBogrim.Products
{
    public class Neckless : IProduct
    {
        private string _name;
        private double _crate;
        private JewleryMaterial _jewleryMaterial;

        public Neckless(string name, double crate, JewleryMaterial jewleryMaterial)
        {
            _name = name;
            _crate = crate;
            _jewleryMaterial = jewleryMaterial;
        }
        public string GetName()
        {
            return _name;
        }

        public string GetProductInformation()
        {
            return $"{_name}, {_crate} Crate, Made from {_jewleryMaterial}";
        }
    }
}
