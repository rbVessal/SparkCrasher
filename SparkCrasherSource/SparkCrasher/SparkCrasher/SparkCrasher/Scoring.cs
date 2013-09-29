using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SparkCrasher
{
    class Scoring
    {
        //Attributes
        Game1 lev = new Game1();
        private List<double> tempList;
        private Dictionary<int, double> scoreList;
        private double[] highScoreList;
        private double finalScore;
        private double temp1;
        private double temp2;

        public Scoring()
        {
            tempList = new List<double>();
            scoreList = new Dictionary<int, double>();//A list of scores for each individual level of the game. To add together
            highScoreList = new double[11];//List to load txt file into and sort.
            finalScore = 0;
        }

        //Properties====================================================================================================================
        public Dictionary<int, double> ScoreList
        {
            get { return scoreList; }
            set { scoreList = value; }
        }
        public double[] HighScoreList
        {
            get { return highScoreList; }
            set { highScoreList = value; }
        }
        public double FinalScore
        {
            get { return finalScore; }
            set { finalScore = value; }
        }



        //Methods

        /// <summary>
        /// Loads the scores from a set document and parses them all to be doubles. Populates the highScoreList
        /// </summary>
        /// <param name="fn"></param>
        public void LoadScores(String fn)//Takes a file name
        {
            //AF-Set up stream reader...Havent done this in a while
            StreamReader input = new StreamReader(fn);

            String line = "";
            int temp = 0;

            while ((line = input.ReadLine()) != null)
            {
                //AF-Adding individual lines to be broken into chars
                HighScoreList[temp] = (Double.Parse(line));
                temp++;
            }

            input.Close();
        }

        /// <summary>
        /// Writes the lowest 11 scores to a text file in order
        /// </summary>
        /// <param name="fn"></param>
        public void WriteScores(String fn)
        {
            StreamReader input = new StreamReader(fn);

            String line = "";

            //Populate the temp list with scores from the text document
            
            
                while ((line = input.ReadLine()) != null)
                {
                    tempList.Add(Double.Parse(line));
                }
            

            //Close strreamreader so stupid things dont happen
            input.Close();

            //Create streamWriter for writing to the text file.
            StreamWriter output = new StreamWriter(fn);

            //Total the final score
            for (int i = 1; i <= ScoreList.Count(); i++)
            {
                FinalScore += ScoreList[i];
            }


            //If the new final score is bigger than the smallest high score
            if (tempList.Count() == 0)
            {
                output.WriteLine(FinalScore);
            }
            else
            {
                if (FinalScore <= tempList[tempList.Count-1])//got rid of -1, might need that back
                {
                    if (tempList.Count() == 11)
                    {
                        tempList[tempList.Count - 1] = FinalScore;

                        //Sorting into list
                        SortScores(tempList);
                    }
                    else
                    {
                        tempList.Add(FinalScore);
                        SortScores(tempList);
                    }
                }
                else if (FinalScore >= tempList[tempList.Count - 1] && tempList.Count() != 11)
                {
                    tempList.Add(FinalScore);
                    SortScores(tempList);
                }
            }

            for (int i = 0; i < tempList.Count(); i++)
            {
                output.WriteLine(tempList[i]);
            }

            output.Close();
        }

        /// <summary>
        /// Sorts scores in designated list
        /// </summary>
        /// <param name="sort"></param>
        public void SortScores(List<double> sort)
        {
            for (int j = 0; j < sort.Count(); j++)
            {
                temp1 = sort[j];

                for (int i = 0; i < sort.Count(); i++)
                {
                    temp2 = sort[i];
                    if (temp1 < temp2)
                    {
                        sort[i] = temp1;
                        sort[j] = temp2;
                        temp1 = temp2;
                    }
                }

            }
        }

    }
}
