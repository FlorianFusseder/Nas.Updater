using System;
using System.Windows.Forms;
using NAS.Updater.Properties;

namespace NAS.Updater
{
    class FileAktualisiererCommand : ICommand
    {
        private FileAktualisierer fileAktualisierer;

        public FileAktualisiererCommand(FileAktualisierer fileAktualisierer)
        {
            this.fileAktualisierer = fileAktualisierer;
        }

        public void Execute()
        {
            try
            {
                fileAktualisierer.Aktualisiere();
            }
            catch (Exception e)
            {
                MessageBox.Show(Resources.FileAktualisiererCommand_Execute_An_exception_occured);
                Logger.WriteFailureLine("");
                Logger.WriteFailureLine($"1. InnerException:    {e.InnerException}");
                Logger.WriteFailureLine($"2. Message:           {e.Message}");
                Logger.WriteFailureLine($"3. Source:            {e.Source}");
                Logger.WriteFailureLine($"4. TargetSite:        {e.TargetSite}");
                Logger.WriteFailureLine("");
            }
        }
    }
}
