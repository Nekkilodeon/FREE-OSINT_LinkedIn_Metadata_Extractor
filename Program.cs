using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FREE_OSINT_Lib;

namespace FREE_OSINT_LinkedIn_Metadata_Extractor
{
    class Program : IGeneral_module, IProcessing_Module
    {
        private string title = "FREE-OSINT LinkedIn Metadata Extractor";
        private string description = "Extracts Name, Surname and Picture URL from profile metadata. Apply only on FREE-OSINT_Google_Custom_Search LinkedIn results";
        private string extracted_node_title = "FREE-OSINT LinkedIn Metadata Extractor";
        static void Main(string[] args)
        {

        }

        public string Description()
        {
            return this.description;
        }

        public TreeNode Process(TreeNode treeNode)
        {
            TreeNode finalNode = new TreeNode(extracted_node_title);
            List<TreeNode> extracted_list = new List<TreeNode>();
            if (treeNode.Nodes.Count > 0)
            {
                foreach (TreeNode sub in treeNode.Nodes)
                {
                    TreeNode extracted;
                    extracted = extract_metadata(sub);
                    if (extracted != null)
                        extracted_list.Add(extracted);
                }
            }

            finalNode.Nodes.AddRange(extracted_list.ToArray());
            return finalNode;
        }

        private TreeNode extract_metadata(TreeNode subnode)
        {
            if (subnode.Nodes.Count > 0)
            {
                TreeNode link = null;
                foreach (TreeNode subsubNode in subnode.Nodes)
                {
                    if (subsubNode.Text.Contains("linkedin.com"))
                    {
                        link = subsubNode;
                    }
                    if (subsubNode.Text.Equals("Metadata"))
                    {
                        String first_name = "";
                        String last_name = "";
                        String image_link = "";
                        foreach (TreeNode meta_node in subsubNode.Nodes)
                        {
                            if (meta_node.Text.Contains("og:image"))
                            {
                                image_link = meta_node.Text.Split(new string[] { ": \"" }, StringSplitOptions.None)[1];
                                image_link = image_link.Remove(image_link.Length - 1);
                            }
                            else if (meta_node.Text.Contains("profile:last_name"))
                            {
                                last_name = meta_node.Text.Split(new string[] { ": \"" }, StringSplitOptions.None)[1];
                                last_name = last_name.Remove(last_name.Length - 1);
                            }
                            else if (meta_node.Text.Contains("profile:first_name"))
                            {
                                first_name = meta_node.Text.Split(new string[] { ": \"" }, StringSplitOptions.None)[1];
                                first_name = first_name.Remove(first_name.Length - 1);
                            }
                        }
                        if(first_name.Length > 1 || last_name.Length > 1)
                        {

                            TreeNode extracted = new TreeNode(first_name + " " + last_name);
                            extracted.Nodes.Add(new TreeNode(first_name));
                            extracted.Nodes.Add(new TreeNode(last_name));
                            extracted.Nodes.Add(new TreeNode(link.Text));
                            extracted.Nodes.Add(new TreeNode(image_link));
                            return extracted;
                        }
                    }
                    else if (subsubNode.Nodes.Count > 0)
                    {
                        return extract_metadata(subsubNode);
                    }
                }
                return null;
            }
            else
            {
                return null;
            }
        }

        public string Title()
        {
            return this.title;
        }
    }
}
