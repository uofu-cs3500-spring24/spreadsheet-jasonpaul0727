// Skeleton implementation written by Joe Zachary for CS 3500, September 2013.
// Version 1.1 (Fixed error in comment for RemoveDependency.)
// Version 1.2 - Daniel Kopta
// (Clarified meaning of dependent and dependee.)
// (Clarified names in solution/project structure.)
/// <summary>
/// Author:    Yanxia Bu
/// Partner:    No
/// Date:     1/23/2024
/// Course:    CS 3500, University of Utah, School of Computing
/// Copyright: CS 3500 and Yanxia Bu - This work may not 
///            be copied for use in Academic Coursework.
///
/// I, Yanxia Bu, certify that I wrote this code from scratch and
/// did not copy it in part or whole from another source.  All 
/// references used in the completion of the assignments are cited 
/// in my README file.
///
/// File Contents
///
///   The dependency Graph 
/// </summary>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace SpreadsheetUtilities
{
    /// <summary>
    /// (s1,t1) is an ordered pair of strings
    /// t1 depends on s1; s1 must be evaluated before t1
    ///
    /// A DependencyGraph can be modeled as a set of ordered pairs of strings. Two
    ///ordered pairs
    /// (s1,t1) and (s2,t2) are considered equal if and only if s1 equals s2 and t1
    ///equals t2.
    /// Recall that sets never contain duplicates. If an attempt is made to add an
    ///element to a
    /// set, and the element is already in the set, the set remains unchanged.
    ///
    /// Given a DependencyGraph DG:
    ///
    /// (1) If s is a string, the set of all strings t such that (s,t) is in DG is
    ///called dependents(s).
    /// (The set of things that depend on s)
    ///
    /// (2) If s is a string, the set of all strings t such that (t,s) is in DG is
    ///called dependees(s).
    /// (The set of things that s depends on)
    //
    // For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d", "d")}
    // dependents("a") = {"b", "c"}
    // dependents("b") = {"d"}
    // dependents("c") = {}
    // dependents("d") = {"d"}
    // dependees("a") = {}
    // dependees("b") = {"a"}
    // dependees("c") = {"a"}
    // dependees("d") = {"b", "d"}
    /// </summary>
    public class DependencyGraph
    {
        /// <summary>
        /// Creates an empty DependencyGraph.
        /// </summary>
        private Dictionary<string, HashSet<string>> dependents;
        private Dictionary<string, HashSet<string>> dependees;
        private int size;
        public DependencyGraph()
        {
            dependents = new Dictionary<string, HashSet<string>>();
            dependees = new Dictionary<string, HashSet<string>>();
        }
        /// <summary>
        ///  /// The number of ordered pairs in the DependencyGraph.
        /// </summary>
        public int Size
        {
            get { return size; }

        }
        /// <summary>
        /// find the value of dependees 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>  integer
        public int this[string s]
        {
            get
            {
                if (dependees.ContainsKey(s))
                {
                    return dependees[s].Count;
                }
                return 0;
            }
        }
        /// <summary>
        /// check wheather has the dependent for string S 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></boolean>
        public bool HasDependents(string s)
        {
            // if dependents dictionary in s index has value and it not empty return true 
            if (dependents.ContainsKey(s))
            {
                return true;
            }
            // otherwise return false 
            return false;
        }
        /// <summary>
        /// check wheather has the dependee for string S 
        /// </summary>
        /// <param name="s"></string>
        /// <returns></bool>
        public bool HasDependees(string s)
        {
            // if dependents dictionary in s index has value and it not empty return true 
            if (dependees.ContainsKey(s))
            {
                if (dependees[s].Count < 1)
                {
                    return false;
                }
                return true;
            }
            // otherwise return false 
            return false;
        }
       /// <summary>
       /// get the dependent for string S otherwise return the empty hashset for string S 
       /// </summary>
       /// <param name="s"></param>
       /// <returns></returns>
        public IEnumerable<string> GetDependents(string s)
        {
            // return the hashset for dependent s or return empty hashset for dependent s 
            if (dependents.ContainsKey(s))
            {
                return new HashSet<string>(dependents[s]);
            }
            return new HashSet<string>();
        }
        /// <summary>
        ///  get the dependees for string S otherwise return the empty hashset for string S 
        /// </summary>
        /// <param name="s"></param>
        /// <returns></Hashset</string>>
        public IEnumerable<string> GetDependees(string s)
        {
            // return the hashset for dependees s or return empty hashset for dependent s 
            if (dependees.ContainsKey(s))
            {
                return new List<string>(dependees[s]);
            }
            return new HashSet<string>(); ;
        }
        /// <summary>
        /// <para>Adds the ordered pair (s,t), if it doesn't exist in dependent we will create the new hashset and add it 
        /// same with dependeee and if both do not set up the hashset we will create new hashsets in  both dictionart
        /// <para>This should be thought of as:</para>
        /// t depends on s
        /// </summary>
        /// <param name="s"> s must be evaluated first. T depends on S</param>
        /// <param name="t"> t cannot be evaluated until s is</param> //.
        public void AddDependency(string s, string t)
        {
            // both dictionary has key and add together with preventing duplicate 
            if (dependents.ContainsKey(s) && dependees.ContainsKey(t))
            {
                if (!dependents[s].Contains(t))
                {
                    dependents[s].Add(t);
                    dependees[t].Add(s);
                    size++;
                }
            }
            // dependent has already has key but dependees not will create new value hashset and dependent just add the value in hashset
            else if (dependents.ContainsKey(s) && !dependees.ContainsKey(t))
            {
                dependees.Add(t, new HashSet<string>());
                dependees[t].Add(s);
                dependents[s].Add(t);
                size++;

            }
            // dependees has already has key but dependents not will create new value hashset and dependees just add the value in hashset
            else if (!dependents.ContainsKey(s) && dependees.ContainsKey(t))
            {
                dependents.Add(s, new HashSet<string>());
                dependents[s].Add(t);
                dependees[t].Add(s);
                size++;
            }
            // if both do not have hashset add  in each and add it 
            else if (!dependents.ContainsKey(s) && !dependees.ContainsKey(t))
            {
                dependents.Add(s, new HashSet<string>());
                dependents[s].Add(t);
                dependees.Add(t, new HashSet<string>());
                dependees[t].Add(s);
                size++;
            }
        }

        /// <summary>
        /// Removes the ordered pair (s,t), if it exists if the hashset is empty in dictionary remove it, and decreasing the size
        /// </summary>
        /// <param name="s"></param>
        /// <param name="t"></param>
        public void RemoveDependency(string s, string t)
        {
            // check whether both dictionary has key and value for preveting wrong format 
            if (dependents.ContainsKey(s) && dependees.ContainsKey(t))
            {
                dependents[s].Remove(t);
                dependees[t].Remove(s);
                // if it is zero remove the hashset in dictionary
                if (dependees[t].Count == 0)
                {
                    dependees.Remove(t);
                }
                if (dependents[s].Count == 0)
                {
                    dependents.Remove(s);
                }
                size--;
            }
        }
        /// <summary>
        /// Removes all existing ordered pairs of the form (s,r). Then, for each
        /// t in newDependents, adds the ordered pair (s,t).
        /// </summary>
        /// <param name="s"></string>
        /// <param name="newDependents"></IEnumerable>
        public void ReplaceDependents(string s, IEnumerable<string> newDependents)
        {
            // replace in dependent dictionary if hashset has value
            if (dependents.ContainsKey(s))
            {

                foreach (string st in dependents[s])
                {
                    RemoveDependency(s, st);
                }
                foreach (string st in newDependents)
                {
                    AddDependency(s, st);
                }

            }
            // no value just add directly
            else
            {
                foreach (string st in newDependents)
                {
                    AddDependency(s, st);
                }
            }
        }
        /// <summary>
        //Removes all existing ordered pairs of the form (r,s). Then, for each
        /// t in newDependees, adds the ordered pair (t,s).
        /// </summary>
        /// <param name="s"></string>
        /// <param name="newDependees"></IEnumerable>
        public void ReplaceDependees(string s, IEnumerable<string> newDependees)
        {

            // replace in dependees dictionary if hashset has value
            if (dependees.ContainsKey(s))
            {
                foreach (string st in dependees[s])
                {
                    RemoveDependency(st, s);
                }
                foreach (string st in newDependees)
                {
                    AddDependency(st, s);
                }
            }
            // no value just add directly
            else
            {
                foreach (string st in newDependees)
                {
                    AddDependency(st, s);
                }
            }
        }
    }
}