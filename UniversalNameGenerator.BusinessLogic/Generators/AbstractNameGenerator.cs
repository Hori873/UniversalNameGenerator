﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using UniversalNameGenerator.BusinessLogic.Generators.Interfaces;

namespace UniversalNameGenerator.BusinessLogic.Generators
{
    public abstract class AbstractNameGenerator : INameGenerator
    {
        /// <summary>
        /// Gets or sets the minimum length of the name.
        /// </summary>
        /// <value>The minimum length of the name.</value>
        public int MinNameLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum length of the name.
        /// </summary>
        /// <value>The maximum length of the name.</value>
        public int MaxNameLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum processing time per word.
        /// </summary>
        /// <value>The maximum processing time per word, in milliseconds.</value>
        public int MaxProcessingTimePerWord { get; set; }

        /// <summary>
        /// Gets or sets the excluded strings.
        /// </summary>
        /// <value>The excluded strings.</value>
        public List<string> ExcludedStrings { get; set; }

        /// <summary>
        /// Gets or sets the included strings.
        /// </summary>
        /// <value>The included strings.</value>
        public List<string> IncludedStrings { get; set; }

        /// <summary>
        /// Gets or sets the string that all generated names must start with.
        /// </summary>
        /// <value>The string start filter.</value>
        public string StartsWithFilter { get; set; }

        /// <summary>
        /// Gets or sets the string that all generated names must end with.
        /// </summary>
        /// <value>The string end filter.</value>
        public string EndsWithFilter { get; set; }

        /// <summary>
        /// Gets or sets the used words.
        /// </summary>
        /// <value>The used words.</value>
        public List<string> GeneratedWords { get; protected set; }

        protected readonly Random random;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractNameGenerator"/> class.
        /// </summary>
        protected AbstractNameGenerator()
        {
            MinNameLength = 5;
            MaxNameLength = 10;
            MaxProcessingTimePerWord = 1000;

            StartsWithFilter = string.Empty;
            EndsWithFilter = string.Empty;

            ExcludedStrings = new List<string>();
            IncludedStrings = new List<string>();
            GeneratedWords = new List<string>();

            random = new Random();
        }

        /// <summary>
        /// Gets a name.
        /// </summary>
        /// <returns>The name.</returns>
        public abstract string GenerateName();

        /// <summary>
        /// Generates names.
        /// </summary>
        /// <returns>The names.</returns>
        /// <param name="maximumCount">Maximum count.</param>
        public List<string> GenerateNames(int maximumCount)
        {
            List<string> names = new List<string>();

            DateTime startTime = DateTime.Now;
            DateTime currentTime = DateTime.Now;
            DateTime endTime = startTime.AddMilliseconds(MaxProcessingTimePerWord * maximumCount);

            while (currentTime < endTime && names.Count < maximumCount)
            {
                string name = GenerateName();

                // The generated name wasn't valid
                if (name != null)
                {
                    names.Add(name);
                }

                currentTime = DateTime.Now;
            }

            return names;
        }

        /// <summary>
        /// Reset the list of used names.
        /// </summary>
        public void Reset()
        {
            GeneratedWords.Clear();
        }

        /// <summary>
        /// Checks wether the the name is valid.
        /// </summary>
        /// <returns><c>true</c>, if name is valid, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        protected bool IsNameValid(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return false;
            }

            if (name.Length < MinNameLength || name.Length > MaxNameLength)
            {
                return false;
            }
            
            if (!name.StartsWith(StartsWithFilter, StringComparison.InvariantCulture) ||
                !name.EndsWith(EndsWithFilter, StringComparison.InvariantCulture))
            {
                return false;
            }

            // The same name was previously generated
            if (GeneratedWords.Contains(name))
            {
                return false;
            }

            // The name contains a blacklisted pattern
            if (ExcludedStrings.Any(p => Regex.IsMatch(name, p)))
            {
                return false;
            }

            // The name does not contain a mandatory pattern
            if (!IncludedStrings.All(p => Regex.IsMatch(name, p)))
            {
                return false;
            }

            return true;
        }
    }
}
