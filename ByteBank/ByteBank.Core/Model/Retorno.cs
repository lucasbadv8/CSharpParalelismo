using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ByteBank.Core.Annotations;

namespace ByteBank.Core.Model
{
    public class Retorno : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string valor;
        public string Valor
        {
            get { return this.valor; }
            set
            {
                if (this.valor != value)
                {
                    this.valor = value;
                    this.OnPropertyChanged("Valor");
                }
            }
        }

        private void NotifyPropertyChanged(string s)
        {
            throw new NotImplementedException();
        }


        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
