using Microsoft.EntityFrameworkCore;
using SaverBackend.Models;
using Flurl.Http;

namespace ContentSorter
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        private ApplicationContext database;
        private LinkedList<Content> content;
        LinkedListNode<Content> current;
        private int currentState = 0;
        private string directory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public MainPage()
        {
            InitializeComponent();
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 21));

            var options = new DbContextOptionsBuilder<ApplicationContext>()
                .UseMySql("Server=192.168.88.252;Database=mobilesdb;Uid=user;Pwd=password;", serverVersion).Options;
            
            this.content = new LinkedList<Content>();

            this.database = new ApplicationContext(options);
            this.Appearing += appearing;
            this.next.Clicked += OnNextClicked;
            this.blondes.Clicked += OnBlondes;
            this.brunettes.Clicked += OnBruneetes;
            this.sluts.Clicked += OnSluts;
            this.milf.Clicked += OnMilf;
            this.cumface.Clicked += OnCumfae;
            this.bj.Clicked += OnBj;
            this.curvys.Clicked += OnCurvys;
            this.Lips.Clicked += OnLips;
            this.ass.Clicked += OnAss;
            this.tits.Clicked += OnTits;
            this.face.Clicked += OnFace;
            this.red.Clicked += OnRed;


            var allContent = this.database.Contents;

            foreach (var content in allContent)
            {
                this.content.AddLast(content);
            }
            RestoreCurrentState();
            this.current = this.content.Find(this.database.Contents.First());
        }

        private async void OnBlondes(object sender, EventArgs e)
        {
            await this.current.Value.ImageUri.DownloadFileAsync($"{this.directory}/data/Blondes");
            this.Next();
        }

        private async void OnBruneetes(object sender, EventArgs e)
        {
            await this.current.Value.ImageUri.DownloadFileAsync($"{this.directory}/data/Brunettes");
            this.Next();
        }

        private async void OnSluts(object sender, EventArgs e)
        {
            await this.current.Value.ImageUri.DownloadFileAsync($"{this.directory}/data/Sluts");
            this.Next();
        }

        private async void OnMilf(object sender, EventArgs e)
        {
            await this.current.Value.ImageUri.DownloadFileAsync($"{this.directory}/data/Milf");
            this.Next();
        }

        private async void OnCumfae(object sender, EventArgs e)
        {
            await this.current.Value.ImageUri.DownloadFileAsync($"{this.directory}/data/Cumface");
            this.Next();
        }

        private async void OnBj(object sender, EventArgs e)
        {
            await this.current.Value.ImageUri.DownloadFileAsync($"{this.directory}/data/Bj");
            this.Next();
        }

        private async void OnCurvys(object sender, EventArgs e)
        {
            await this.current.Value.ImageUri.DownloadFileAsync($"{this.directory}/data/Curvys");
            this.Next();
        }

        private async void OnLips(object sender, EventArgs e)
        {
            await this.current.Value.ImageUri.DownloadFileAsync($"{this.directory}/data/Lips");
            this.Next();
        }

        private async void OnAss(object sender, EventArgs e)
        {
            await this.current.Value.ImageUri.DownloadFileAsync($"{this.directory}/data/Ass");
            this.Next();
        }

        private async void OnTits(object sender, EventArgs e)
        {
            await this.current.Value.ImageUri.DownloadFileAsync($"{this.directory}/data/Tits");
            this.Next();
        }

        private async void OnFace(object sender, EventArgs e)
        {
            await this.current.Value.ImageUri.DownloadFileAsync($"{this.directory}/data/Face");
            this.Next();
        }

        private async void OnRed(object sender, EventArgs e)
        {
            await this.current.Value.ImageUri.DownloadFileAsync($"{this.directory}/data/Red");
            this.Next();
        }

        public void RestoreCurrentState() 
        {
            var p = Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"state.txt"));
            var f = File.ReadAllText(p);
            this.currentState = int.Parse(f);
        }

        private void Next() 
        {
            var nextCurrent = this.current.Next;
            this.image.Source = this.current.Next.Value.ImageUri;
            this.current = nextCurrent;
            this.StoreCurrentState();
        }

        private void OnNextClicked(object sender, EventArgs e)
        {
            var nextCurrent = this.current.Next;
            this.image.Source = this.current.Next.Value.ImageUri;
            this.current = nextCurrent;
            this.StoreCurrentState();
        }

        private void StoreCurrentState() 
        {
            // get index of current content
            // get or create a file where to store the value
            // save the value in the file

            using (StreamWriter outputFile = new StreamWriter(Path.Combine(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"state.txt")), false))
            {
                outputFile.WriteLine($"{this.current.Value.Id}");
            }
        }

        private void appearing(object sender, EventArgs e)
        {
            this.current = this.content.Find(this.database.Contents.Single(i => i.Id == this.currentState));
            this.image.Source = current.Value.ImageUri;

        }
    }
}
