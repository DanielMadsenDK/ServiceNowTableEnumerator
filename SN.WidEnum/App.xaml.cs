namespace SN.WidEnum;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new AppShell();
	}

	protected override Window CreateWindow(IActivationState activationState)
    {
        var window = base.CreateWindow(activationState);
        if (window != null)
        {
            window.Title = "ServiceNow Widget Enum by Daniel Madsen ( zdndk )";
            window.Width = 500;
            window.Height = 800;
        }
        return window;
    }
}
