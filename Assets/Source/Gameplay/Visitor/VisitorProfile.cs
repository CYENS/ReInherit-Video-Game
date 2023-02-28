using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cyens.ReInherit
{
    public class VisitorProfile
    {
        private string m_name;
        private string m_sex;
        private string m_nationality;
        private List<string> m_interests; // List of string interests (e.g. "paintings', "historical", ...)

        public string GetName()
        {
            return m_name;
        }
        public string GetSex()
        {
            return m_sex;
        }
        public string GetNationality()
        {
            return m_nationality;
        }
        public void AppendInterest(string interest)
        {
            m_interests.Add(interest);
        }

        public VisitorProfile(string name, string sex, string nationality, List<string> interests)
        {
            m_name = name;
            m_sex = sex;
            m_nationality = nationality;
            m_interests = interests;
        }

        public void ConstructVisitor()
        {
            // Combine clothes, skin colors etc to construct visitor character
        }
    }
}
