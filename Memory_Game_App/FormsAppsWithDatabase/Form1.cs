using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FormsAppsWithDatabase
{
    public partial class Form1 : Form
    {
        private bool allowClick = false; //to not let the player click too fast on the images! (controlling when they can click)
        private int counter = 0; //to count the number of matches
        private int tries = 0;  //to count the number of tries for the player
        Stopwatch elapsedTime = new Stopwatch(); //to count elapsed time for the game
        //Random image selector
        Random rnd = new Random();
        //We will have all the pictureBoxes into an array
        private PictureBox[] pictureBoxes
        {
            get { return Controls.OfType<PictureBox>().ToArray(); }
        }
        //Image getter (array) for random placement
        private static IEnumerable<Image> images
        {
            get
            {
                return new Image[]
                {
                    Properties.Resources.basketball,
                    Properties.Resources.book,
                    Properties.Resources.fence,
                    Properties.Resources.film,
                    Properties.Resources.hdd,
                    Properties.Resources.paint,
                    Properties.Resources.shoe,
                    Properties.Resources.throphy,
                    Properties.Resources.tools,
                    Properties.Resources.wheel
                };
            }
        }
        PictureBox firstClicked, secondClicked;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            counter = 0; //initialize counter of pairs
            tries = 0; //initialize efforts
        }

        //We set all pair of images into the free slots (and check if they have the same tag)
        private void setRandomImages()
        {
            foreach (var image in images)
            {
                getFreeSlot().Tag = image;
                getFreeSlot().Tag = image;
            }
        }

        //For returning the empty slots back to the program for set up
        private PictureBox getFreeSlot()
        {
            int num;
            do
            {
                num = rnd.Next(0, pictureBoxes.Count());
            } while (pictureBoxes[num].Tag != null);

            return pictureBoxes[num];
        }

        private void HideImages()
        {
            foreach (var pic in pictureBoxes)
            {
                pic.Image = Properties.Resources.mystery;
            }
        }        

        private void startGame(object sender, EventArgs e)
        {
            setRandomImages(); //set random image positions
            HideImages(); //hide all images with the mystery icon
            allowClick = true;
            button1.Enabled = false; //disable the "Start Game" button
            elapsedTime.Start();
        }

        private void clickImage(object sender, EventArgs e)
        {
            //If click is not allowed, return
            if (!allowClick) return;

            PictureBox pic = sender as PictureBox;

            //Check if picture was clicked and is not null
            if (pic == null) return;

            //First image found
            if (firstClicked == null)
            {
                firstClicked = pic;
                pic.Image = (Image)pic.Tag;
                return;
            }

            //Second image found
            secondClicked = pic;
            pic.Image = (Image)pic.Tag;

            //Check if second image clicked is the same as the first one (but not the first one selected two times!)
            if ((secondClicked.Image == firstClicked.Image) && (secondClicked != firstClicked))
            {
                //Count effort of player
                tries += 1;
                firstClicked.Visible = secondClicked.Visible = true; //let the paired pictures be visible from now on
                firstClicked.Enabled = secondClicked.Enabled = false; //to not let the same pictures trigger the matching
                counter += 2; //we count with a step of 2 because we match PAIRS!
                firstClicked = null;
                secondClicked = null;
            }
            else
            {
                //Count effort of player
                tries += 1;
                allowClick = false;
                timer1.Start();
            }

            //Check if the game was won
            CheckForWinner();
            
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //Wait for a second and then hide the unmatched pictures
            firstClicked.Image = Properties.Resources.mystery;
            secondClicked.Image = Properties.Resources.mystery;

            firstClicked = null;
            secondClicked = null;

            timer1.Stop();
            allowClick = true;
        }

        private void CheckForWinner()
        {
            //All images have been matched!
            if (counter == pictureBoxes.Length)
            {
                elapsedTime.Stop();
                long duration = elapsedTime.ElapsedMilliseconds/1000; //convert to seconds
                MessageBox.Show("You won!\nNumber of tries: "+ tries + "\nElapsed Time: " + duration + " seconds\n");
                //Close the application if game is won
                Environment.Exit(0);
            }
        }
        
    }
}
