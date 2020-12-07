﻿using System;
using System.Collections.Generic;
using System.Text;
using TabloidCLI.Models;

namespace TabloidCLI.UserInterfaceManagers
{
    public class BlogManager : IUserInterfaceManager
    {
        private readonly IUserInterfaceManager _parentUI;
        private BlogRepository _blogRepository;
        private string _connectionString;


        public BlogManager(IUserInterfaceManager parentUI, string connectionString)
        {
            _parentUI = parentUI;
            _blogRepository = new BlogRepository(connectionString);
            _connectionString = connectionString;
        }


        public IUserInterfaceManager Execute()
        {
            Console.WriteLine("Blog Menu");

            Console.WriteLine("1) Add a blog");
            Console.WriteLine("2) List all blogs");
            Console.WriteLine("3) Delete a blog");
            Console.WriteLine("4) Edit a blog");

            Console.Write("> ");
            string choice = Console.ReadLine();
            switch (choice)
            {
                case "1":
                    Add();
                    return this; 
                case "2":
                    throw new NotImplementedException();
                case "3": 
                    throw new NotImplementedException();
                case "4": 
                    throw new NotImplementedException();

                case "0":
                    Console.WriteLine("Good bye");
                    return null;
                default:
                    Console.WriteLine("Invalid Selection");
                    return this;
            }

           
        }

        private void Add()
        {
            Console.WriteLine("New Blog");
            Blog blog = new Blog();

            Console.Write("Enter Blog Title:");
            blog.Title = Console.ReadLine();

            Console.Write("Enter Blog URL:");
            blog.Url = Console.ReadLine();

            _blogRepository.Insert(blog);
        }

    }
}