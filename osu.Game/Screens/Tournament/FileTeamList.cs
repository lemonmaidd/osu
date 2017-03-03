﻿// Copyright (c) 2007-2017 ppy Pty Ltd <contact@ppy.sh>.
// Licensed under the MIT Licence - https://raw.githubusercontent.com/ppy/osu/master/LICENCE

using osu.Framework.Logging;
using osu.Framework.Platform;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace osu.Game.Screens.Tournament
{
    public class FileTeamList : ITeamList
    {
        private const string teams_filename = "drawings.txt";

        private Storage storage;

        public FileTeamList(Storage storage)
        {
            this.storage = storage;
        }

        public IEnumerable<Team> Teams
        {
            get
            {
                List<Team> teams = new List<Team>();

                try
                {
                    using (Stream stream = storage.GetStream(teams_filename, FileAccess.Read, FileMode.Open))
                    using (StreamReader sr = new StreamReader(stream))
                    {
                        while (sr.Peek() != -1)
                        {
                            string line = sr.ReadLine().Trim();

                            if (string.IsNullOrEmpty(line))
                                continue;

                            string[] split = line.Split(':');

                            if (split.Length < 2)
                            {
                                Logger.Log($"Invalid team definition: {line}. Expected \"flag_name : team_name : team_acronym\".");
                                continue;
                            }

                            string flagName = split[0].Trim();
                            string teamName = split[1].Trim();

                            string acronym = split.Length >= 3 ? split[2].Trim() : teamName;
                            acronym = acronym.Substring(0, Math.Min(3, acronym.Length));

                            teams.Add(new Team()
                            {
                                FlagName = flagName,
                                FullName = teamName,
                                Acronym = acronym
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Failed to read teams.");
                }

                return teams;
            }
        }
    }
}
