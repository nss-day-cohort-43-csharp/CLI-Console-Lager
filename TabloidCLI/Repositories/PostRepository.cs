﻿using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using TabloidCLI.Models;

namespace TabloidCLI.Repositories
{
    public class PostRepository : DatabaseConnector, IRepository<Post>
    {
        public PostRepository(string connectionString) : base(connectionString) { }

        public List<Tag> GetTagsA(int x)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT AuthorTag.AuthorId, Tag.Id AS TagId, Tag.Name
                                        FROM AuthorTag
                                        LEFT JOIN Tag ON TagId = Tag.Id
                                        WHERE AuthorId = @x";
                    cmd.Parameters.AddWithValue("@x", x);

                    List<Tag> tags = new List<Tag>();

                    SqlDataReader reader = cmd.ExecuteReader();
                    Tag tag = null;
                    while (reader.Read())
                    {
                        tag = new Tag()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("TagId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        };
                        tags.Add(tag);
                    }
                    reader.Close();
                    return tags;
                }
            }
        }
        public List<Tag> GetTagsB(int x)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT BlogTag.BlogId, Tag.Id AS TagId, Tag.Name
                                        FROM BlogTag
                                        LEFT JOIN Tag ON TagId = Tag.Id
                                        WHERE BlogId = @x";
                    cmd.Parameters.AddWithValue("@x", x);

                    List<Tag> tags = new List<Tag>();

                    SqlDataReader reader = cmd.ExecuteReader();
                    Tag tag = null;
                    while (reader.Read())
                    {
                        tag = new Tag()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("TagId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                        };
                        tags.Add(tag);
                    }
                    reader.Close();
                    return tags;
                }
            }
        }

        public List<Post> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Post.Id AS PostID, 
                                        Post.Title AS PostTitle, 
                                        Post.URL AS PostUrl, 
                                        Post.PublishDateTime,
                                        Author.Id AS AuthorId,
                                        Author.FirstName, 
                                        Author.LastName,
                                        Author.Bio,
                                        Blog.Title AS BlogTitle,
                                        Blog.Id AS BlogId,
                                        Blog.Url AS BlogUrl
                                        FROM Post
                                        JOIN Author on Author.Id = Post.AuthorId
                                        JOIN Blog on Blog.Id = Post.BlogId";

                    List<Post> posts = new List<Post>();

                    SqlDataReader reader = cmd.ExecuteReader();
                    Post post = null;
                    while (reader.Read())
                    {
                        post = new Post()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("PostId")),
                            Title = reader.GetString(reader.GetOrdinal("PostTitle")),
                            Url = reader.GetString(reader.GetOrdinal("PostUrl")),
                            PublishDateTime = reader.GetDateTime(reader.GetOrdinal("PublishDateTime")),
                            Author = new Author()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("AuthorId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Bio = reader.GetString(reader.GetOrdinal("Bio")),
                                Tags = GetTagsA(reader.GetInt32(reader.GetOrdinal("AuthorId")))
                            },
                            Blog = new Blog()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("BlogId")),
                                Title = reader.GetString(reader.GetOrdinal("BlogTitle")),
                                Url = reader.GetString(reader.GetOrdinal("BlogUrl")),
                                Tags = GetTagsB(reader.GetInt32(reader.GetOrdinal("BlogId")))
                            }
                        };

                        posts.Add(post);
                    }
                    reader.Close();
                    return posts;
                }
            }
        }

        public Post Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Post.Id AS PostID, 
                                        Post.Title AS PostTitle, 
                                        Post.URL AS PostUrl, 
                                        Post.PublishDateTime,
                                        Author.Id AS AuthorId,
                                        Author.FirstName, 
                                        Author.LastName,
                                        Author.Bio,
                                        t.Id AS TagId,
                                        t.Name,
                                        Blog.Title AS BlogTitle,
                                        Blog.Id AS BlogId,
                                        Blog.Url AS BlogUrl
                                        FROM Post
                                        JOIN Author on Author.Id = Post.AuthorId
                                        JOIN Blog on Blog.Id = Post.BlogId
                                        LEFT JOIN PostTag pt on Post.Id = pt.PostId
                                        LEFT JOIN Tag t on t.Id = pt.TagId
                                        WHERE Post.Id = @id";

                    cmd.Parameters.AddWithValue("@id", id);

                    Post post = null;

                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (post == null)
                        {
                            post = new Post()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("PostId")),
                                Title = reader.GetString(reader.GetOrdinal("PostTitle")),
                                Url = reader.GetString(reader.GetOrdinal("PostUrl")),
                                PublishDateTime = reader.GetDateTime(reader.GetOrdinal("PublishDateTime")),
                                Author = new Author()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("AuthorId")),
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    Bio = reader.GetString(reader.GetOrdinal("Bio")),
                                    Tags = GetTagsA(reader.GetInt32(reader.GetOrdinal("AuthorId")))
                                },
                                Blog = new Blog()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("BlogId")),
                                    Title = reader.GetString(reader.GetOrdinal("BlogTitle")),
                                    Url = reader.GetString(reader.GetOrdinal("BlogUrl")),
                                    Tags = GetTagsB(reader.GetInt32(reader.GetOrdinal("BlogId")))
                                }
                            };
                        }
                        if (!reader.IsDBNull(reader.GetOrdinal("TagId")))
                        {
                            post.Tags.Add(new Tag()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("TagId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                            });
                        }
                    }
                    reader.Close();
                    return post;
                }
            }
        }

        public List<Post> GetByAuthor(int authorId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT p.id,
                                               p.Title As PostTitle,
                                               p.URL AS PostUrl,
                                               p.PublishDateTime,
                                               p.AuthorId,
                                               p.BlogId,
                                               a.FirstName,
                                               a.LastName,
                                               a.Bio,
                                               b.Title AS BlogTitle,
                                               b.URL AS BlogUrl
                                          FROM Post p 
                                               LEFT JOIN Author a on p.AuthorId = a.Id
                                               LEFT JOIN Blog b on p.BlogId = b.Id 
                                         WHERE p.AuthorId = @authorId";
                    cmd.Parameters.AddWithValue("@authorId", authorId);
                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Post> posts = new List<Post>();
                    while (reader.Read())
                    {
                        Post post = new Post()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Title = reader.GetString(reader.GetOrdinal("PostTitle")),
                            Url = reader.GetString(reader.GetOrdinal("PostUrl")),
                            PublishDateTime = reader.GetDateTime(reader.GetOrdinal("PublishDateTime")),
                            Author = new Author()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("AuthorId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Bio = reader.GetString(reader.GetOrdinal("Bio")),
                            },
                            Blog = new Blog()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("BlogId")),
                                Title = reader.GetString(reader.GetOrdinal("BlogTitle")),
                                Url = reader.GetString(reader.GetOrdinal("BlogUrl")),
                            }
                        };
                        posts.Add(post);
                    }

                    reader.Close();

                    return posts;
                }
            }
        }

        public void Insert(Post post)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Post (Title, URL, PublishDateTime, AuthorId, BlogId)
                                         VALUES (@t, @u, @pdt, @a, @b)";
                    cmd.Parameters.AddWithValue("@t", post.Title);
                    cmd.Parameters.AddWithValue("@u", post.Url);
                    cmd.Parameters.AddWithValue("@pdt", post.PublishDateTime);
                    cmd.Parameters.AddWithValue("@a", post.Author.Id);
                    cmd.Parameters.AddWithValue("@b", post.Blog.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Update(Post post)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE Post
                                        SET Title = @t,
                                        URL = @u,
                                        PublishDateTime = @pdt,
                                        AuthorId = @aid,
                                        BlogId = @bid
                                        WHERE id = @id";

                    cmd.Parameters.AddWithValue("@t", post.Title);
                    cmd.Parameters.AddWithValue("@u", post.Url);
                    cmd.Parameters.AddWithValue("@pdt", post.PublishDateTime);
                    cmd.Parameters.AddWithValue("@aid", post.Author.Id);
                    cmd.Parameters.AddWithValue("@bid", post.Blog.Id);
                    cmd.Parameters.AddWithValue("@id", post.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM PostTag WHERE PostId = @id2";
                    cmd.Parameters.AddWithValue("@id2", id);
                    cmd.ExecuteNonQuery();
                    cmd.CommandText = @"DELETE FROM Post WHERE id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void InsertTag(Post post, Tag tag)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO PostTag (PostId, TagId)
                                                       VALUES (@postId, @tagId)";
                    cmd.Parameters.AddWithValue("@postId", post.Id);
                    cmd.Parameters.AddWithValue("@tagId", tag.Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteTag(int postId, int tagId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE FROM PostTag
                                         WHERE PostId = @postId AND 
                                               TagId = @tagId";
                    cmd.Parameters.AddWithValue("@postId", postId);
                    cmd.Parameters.AddWithValue("@tagId", tagId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
