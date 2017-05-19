using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SapphireActorCapture.UI
{
    public partial class MapViewForm : Form
    {

        private List<Packets.Receive.ActorSpawnPacket> actors = new List<Packets.Receive.ActorSpawnPacket>();
        private int playerX, playerY;
        private int territoryId;

        public MapViewForm()
        {
            InitializeComponent();

            infoLabel.Parent = mapPictureBox;

            mapPictureBox.Image = Properties.Resources.error;
            mapPictureBox.Size = new Size(Properties.Resources.error.Width / 4, Properties.Resources.error.Height / 4);
            mapPictureBox.Paint += drawMap;

            this.Width = mapPictureBox.Size.Width + 16;
            this.Height = mapPictureBox.Size.Height + 38;

            //SetMapWithId(132, 00);
        }

        private void drawMap(object sender, PaintEventArgs e)
        {
            drawFFXIVCoordDot(playerX, playerY, Color.Green, territoryId);

            drawActors();
        }

        public void SetMapWithId(int id, int submap)
        {
            Console.WriteLine($"MapViewForm: SetMapWithId({id}, 0{submap.ToString()});");

            Models.Territory territory = Globals.exdreader.GetTerritory(id);
            territoryId = id;

            actors.Clear();

            if(territory != null)
            {
                Image mapImage;
                try
                {
                    string path = $"map/ui/map/{territory.Identifier}/0{submap.ToString()}/{territory.Identifier}0{submap.ToString()}_m.tex.png";
                    mapImage = Image.FromFile(path);

                    mapPictureBox.Image = mapImage;
                    mapPictureBox.Size = new Size(mapImage.Width / 3, mapImage.Height / 3);
                    
                    this.Width = mapPictureBox.Size.Width + 16;
                    this.Height = mapPictureBox.Size.Height + 38;

                    infoLabel.Text = $"TerritoryID: {id}\nTerritoryName: {Globals.exdreader.GetTerritoryName(id)}\n{path}";

                    this.Text = $"FFXIVActorCapture | {Globals.exdreader.GetTerritoryName(id)}({id}, 0{submap.ToString()})";
                }
                catch (System.IO.FileNotFoundException exc)
                {
                    if (submap < 4)
                    {
                        SetMapWithId(id, submap + 1);
                    }else
                    {
                        infoLabel.Text = $"TerritoryID: {id}\nTerritoryName: {Globals.exdreader.GetTerritoryName(id)}\nCould not load map file!\n{exc}";

                        mapPictureBox.Image = Properties.Resources.error;
                        mapPictureBox.Size = new Size(Properties.Resources.error.Width / 4, Properties.Resources.error.Height / 4);

                        this.Width = mapPictureBox.Size.Width + 16;
                        this.Height = mapPictureBox.Size.Height + 38;

                        this.Text = $"FFXIVActorCapture | {Globals.exdreader.GetTerritoryName(id)}({id})";
                        return;
                    }
                }
            }
            else
            {
                infoLabel.Text = $"TerritoryID: {id}\nTerritoryName:\nTerritoryID not in EXH!";
                this.Text = $"FFXIVActorCapture | {Globals.exdreader.GetTerritoryName(id)}({id}, 0{submap.ToString()})";

                mapPictureBox.Image = Properties.Resources.error;
                mapPictureBox.Size = new Size(Properties.Resources.error.Width / 4, Properties.Resources.error.Height / 4);

                this.Width = mapPictureBox.Size.Width + 16;
                this.Height = mapPictureBox.Size.Height + 38;
            }
        }

        private void Close(object sender, FormClosingEventArgs e)
        {
            Environment.Exit(0);
        }
        
        public void invalidateMap()
        {
            mapPictureBox.Invalidate();
        }

        public void drawFFXIVCoordDot(int x, int y, System.Drawing.Color color, int territoryId)
        {
            Models.Map myMap = Globals.exdreader.GetMap(territoryId);

            if(myMap != null)
            {
                int a = (mapPictureBox.Height / 2) + (x / (System.Convert.ToInt32(myMap.sizeFactor) / 100));
                int b = (mapPictureBox.Height / 2) + (y / (System.Convert.ToInt32(myMap.sizeFactor) / 100));

                System.Drawing.Graphics graphics = mapPictureBox.CreateGraphics();
                System.Drawing.Rectangle rectangle = new System.Drawing.Rectangle(
                   a, b, 7, 7);
                graphics.FillEllipse(new System.Drawing.SolidBrush(color), rectangle);
            }
        }

        public void drawActors()
        {
            foreach(Packets.Receive.ActorSpawnPacket actor in actors)
            {
                Console.WriteLine("Drawing " + actor.id);
                try
                {
                    drawFFXIVCoordDot(System.Convert.ToInt32(actor.posx), System.Convert.ToInt32(actor.posy), Color.Red, territoryId);

                }catch(Exception exc)
                {
                    Console.Write(exc.GetType().ToString());
                }
            }
        }

        public void addActor(Packets.Receive.ActorSpawnPacket actor)
        {
            Console.WriteLine("Added " + actor.id);
            actors.Add(actor);
        }

        public void setPos(int x, int y)
        {
            playerX = x;
            playerY = y;
        }
    }
}
