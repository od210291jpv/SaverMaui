using BananasGambler.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BananasGambler.ViewModels
{
    internal class PlayGameViewModel : BaseViewModel
    {
        public int ValueToPlay { get; set; }

        private string valueOne { get; set; }
        private string valueTwo { get; set; }
        private string valueThree { get; set; }

        public string ValueOne 
        {
            set 
            {
                this.valueOne = value;
                this.OnPropertyChanged(nameof(this.ValueOne));
            }
            get => this.valueOne;
        }

        public string ValueTwo
        {
            set
            {
                this.valueTwo = value;
                this.OnPropertyChanged(nameof(this.ValueTwo));
            }
            get => this.valueTwo;
        }

        public string ValueThree
        {
            set
            {
                this.valueThree = value;
                this.OnPropertyChanged(nameof(this.ValueThree));
            }
            get => this.valueThree;
        }

        private bool btnOneEnabled { get; set; }

        private bool btnTwoEnabled { get; set; }

        private bool btnThreeEnabled { get; set; }

        public bool BtnOneEnabled 
        {
            get => this.btnOneEnabled;
            set 
            {
                this.btnOneEnabled = value;
                this.OnPropertyChanged(nameof(this.BtnOneEnabled));
            }
        }
        public bool BtnTwoEnabled 
        {
            get => this.btnTwoEnabled;
            set 
            {
                this.btnTwoEnabled = value;
                this.OnPropertyChanged(nameof(this.BtnTwoEnabled));
            }
        }

        public bool BtnThreeEnabled 
        {
            get => this.btnThreeEnabled;
            set 
            {
                this.btnThreeEnabled = value;
                this.OnPropertyChanged(nameof(this.BtnThreeEnabled));
            }
        }

        private string status { get; set; }

        public string Status 
        {
            get => this.status;
            set 
            {
                this.status = value;
                OnPropertyChanged(nameof(this.Status));
            }
        }

        public ICommand PlayCommand { get; set; }

        public ICommand PlayValueOneCommand { get; set; }

        public ICommand PlayValueTwoCommand { get; set; }

        public ICommand PlayValueThreeCommand { get; set; }

        public ICommand BuyOneCommand { get; set; }

        public ICommand BuyThreeCommand { get; set; }

        public ICommand BuyFiveCommand { get; set; }

        public ICommand BuySevenCommand { get; set; }

        public PlayGameViewModel()
        {
            this.InitGameValues();
            this.PlayCommand = new PlayGameCommand(this);
            this.PlayValueOneCommand = new PlayValueOneCommand(this);
            this.PlayValueTwoCommand = new PlayValueTwoCommand(this);
            this.PlayValueThreeCommand = new PlayValueThreeCommand(this);
            this.BuyOneCommand = new ByOneCommand(this);
            this.BuyThreeCommand = new BuyThreeCommand(this);
            this.BuyFiveCommand = new BuyFiveCommand(this);
            this.BuySevenCommand = new BuySevenCommand(this);

            this.Status = "Game Not Started";

            this.BtnThreeEnabled = false;
            this.btnTwoEnabled = false;
            this.btnThreeEnabled = false;
        }

        private void InitGameValues() 
        {
            this.ValueOne = new Random().Next(1, 7).ToString();
            this.ValueTwo = new Random().Next(2, 5).ToString();
            this.ValueThree = new Random().Next(3, 5).ToString();
        }

        public void LockGameButtons(bool unLock) 
        {
            this.btnOneEnabled = unLock;
            this.btnTwoEnabled = unLock;
            this.btnThreeEnabled = unLock;
        }
    }
}
