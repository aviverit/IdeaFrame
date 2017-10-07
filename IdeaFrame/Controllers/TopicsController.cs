using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.Mvc;

using ASP.NET_MVC_Application.Models;
using Newtonsoft.Json;

namespace IdeaFrame.Controllers
{
    public class TopicsController : Controller
    {
        // GET: Topics
        public ActionResult Index()
        {
            var topics = Topic.GetTopic();
            return View(topics);
        }

        public ActionResult Create()
        {
            ViewBag.Submitted = false;
            var created = false;
            // Create the Topic
            if (HttpContext.Request.RequestType == "POST")
            {
                ViewBag.Submitted = true;
                // If the request is POST, get the values from the form
                var id = Request.Form["id"];
                var name = Request.Form["name"];
                var address = Request.Form["address"];
                var trusted = false;

                if (Request.Form["trusted"] == "on")
                {
                    trusted = true;
                }

                // Create a new Topic for these details.
                Topic topic = new Topic()
                {
                    ID = Convert.ToInt16(id),
                    Name = name,
                    Address = address,
                    Trusted = Convert.ToBoolean(trusted)
                };

                // Save the topic in the TopicList
                var TopicFile = Topic.TopicFile;
                var TopicData = System.IO.File.ReadAllText(TopicFile);
                List<Topic> TopicList = new List<Topic>();
                TopicList = JsonConvert.DeserializeObject<List<Topic>>(TopicData);

                if (TopicList == null)
                {
                    TopicList = new List<Topic>();
                }
                TopicList.Add(topic);

                // Now save the list on the disk
                System.IO.File.WriteAllText(TopicFile, JsonConvert.SerializeObject(TopicList));

                // Denote that the topic was created
                created = true;
            }

            if (created)
            {
                ViewBag.Message = "Topic was created successfully.";
            }
            else
            {
                ViewBag.Message = "There was an error while creating the topic.";
            }
            return View();
        }

        public ActionResult Update(int id)
        {
            if (HttpContext.Request.RequestType == "POST")
            {
                // Request is Post type; must be a submit
                var name = Request.Form["name"];
                var address = Request.Form["address"];
                var trusted = Request.Form["trusted"];

                // Get all of the topics
                var topics = Topic.GetTopics();

                foreach (Topic topic in topics)
                {
                    // Find the topic
                    if (topic.ID == id)
                    {
                        // Topic found, now update his properties and save it.
                        topic.Name = name;
                        topic.Address = address;
                        topic.Trusted = Convert.ToBoolean(trusted);
                        // Break through the loop
                        break;
                    }
                }

                // Update the topics in the disk
                System.IO.File.WriteAllText(Topic.TopicFile, JsonConvert.SerializeObject(topics));

                // Add the details to the View
                Response.Redirect("~/Topic/Index?Message=Topic_Updated");
            }


            // Create a model object.
            var clnt = new Topic();
            // Get the list of topics
            var topics = Topic.GetTopics();
            // Search within the topics
            foreach (Topic topic in topics)
            {
                // If the topic's ID matches
                if (topic.ID == id)
                {
                    clnt = topic;
                    // No need to further run the loop 
                    break;
                }
            }
            if (clnt == null)
            {
                // No topic was found
                ViewBag.Message = "No topic was found.";
            }
            return View(clnt);
        }

        public ActionResult Delete(int id)
        {
            // Get the topics
            var Topics = Topic.GetTopics();
            var deleted = false;
            // Delete the specific one.
            foreach (Topic topic in Topics)
            {
                // Found the topic
                if (topic.ID == id)
                {
                    // delete this topic
                    var index = Topics.IndexOf(topic);
                    Topics.RemoveAt(index);

                    // Removed now save the data back.
                    System.IO.File.WriteAllText(Topic.TopicFile, JsonConvert.SerializeObject(Topics));
                    deleted = true;
                    break;
                }
            }

            // Add the process details to the ViewBag
            if (deleted)
            {
                ViewBag.Message = "Topic was deleted successfully.";
            }
            else
            {
                ViewBag.Message = "There was an error while deleting the topic.";
            }
            return View();
        }
    }
}