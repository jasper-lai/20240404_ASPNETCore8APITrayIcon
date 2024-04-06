namespace ASPNETCore8APITrayIcon
{
    public class TrayApplicationContext : ApplicationContext
    {
        private readonly NotifyIcon trayIcon;
        private readonly IHost _appHost;

        public TrayApplicationContext(IHost host)
        {
            _appHost = host;

            Icon myIcon = new Icon("icons/my_tray_icon.ico");

            // Create and configure the tray icon
            trayIcon = new NotifyIcon
            {
                Icon = myIcon,
                Text = "ASPNETCore8APITrayIcon", // Default tooltip text
                Visible = true,
                ContextMenuStrip = new ContextMenuStrip()
            };
            trayIcon.ContextMenuStrip.Items.Add("Exit", null, (sender, e) => ExitApplication());

            // 執行背景服務
            Task.Run(() => _appHost.StartAsync());
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                trayIcon?.Dispose();
            }
            base.Dispose(disposing);
        }

        private async void ExitApplication()
        {
            trayIcon.Visible = false;
            await _appHost.StopAsync();
            Application.Exit();
        }
    }
}
