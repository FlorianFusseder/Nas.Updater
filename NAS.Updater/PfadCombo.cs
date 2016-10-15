using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows.Forms;

namespace NAS.Updater
{
    [DataContract]
    class SpeicherPfadCombo
    {

        [DataMember]
        public string Lokal { get; set; }

        [DataMember]
        public string Nas { get; set; }

        public bool Check()
        {
            if (!Directory.Exists(Lokal))
            {
                MessageBox.Show($"Der Ordner {Lokal} existiert nicht. Das gewünschte Backup wird für diesen ordner nicht ausgeführt", "Nas.Updater hat ein Problem festgestellt");
                return false;
            }
            else if (!Directory.Exists(Nas))
            {
                var mess = MessageBox.Show($"Es existiert kein Backup Ordner auf der NAS mit dem Namen: {Path.GetFileName(Nas)}.\nJetzt erstellen und erneut versuchen?", "Kein NAS Ordner vorhanden", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1, MessageBoxOptions.ServiceNotification);

                if (mess == DialogResult.Yes)
                {
                    try
                    {
                        Directory.CreateDirectory(Nas);
                    }
                    catch (Exception)
                    {
                        MessageBox.Show($"Der Pfad konnte nicht erstellt werden, eventuell fehlt ein Überordner? Backup wird für {Lokal} wird nicht ausgeführt");
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public bool Equals(SpeicherPfadCombo a)
        {
            if ((object)a == null)
                return false;

            return a.Lokal == Lokal && a.Nas == Nas;
        }

        public override bool Equals(object obj)
        {
            var a = obj as SpeicherPfadCombo;

            if (a == null)
                return false;

            return a.Lokal == Lokal && a.Nas == Nas;
        }

        public override int GetHashCode()
        {
            return Lokal.GetHashCode() + Nas.GetHashCode();
        }

        public static bool operator ==(SpeicherPfadCombo a, SpeicherPfadCombo b)
        {
            if (ReferenceEquals(a, b))
                return true;

            if ((object)a == null || (object)b == null)
                return false;

            return a.Lokal == b.Lokal && a.Nas == b.Nas;
        }

        public static bool operator !=(SpeicherPfadCombo a, SpeicherPfadCombo b) => !(a == b);

    }
}
