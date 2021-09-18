using App.Views;

namespace App.Lib
{
    public abstract class RootViewBase : ViewBase
    {
        protected IParameter Parameter;

        public void SetParameter(IParameter parameter)
        {
            Parameter = parameter;
        }

        protected IParameter GetParameter()
        {
            return Parameter;
        }
    }
}