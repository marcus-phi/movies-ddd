module Movies.Infra.Services

open System
open Movies.Domain.Models
open Movies.Domain.Outbound
open TMDbLib.Client
open TMDbLib.Objects.Movies
open TMDbLib.Objects.Search

type TheMoviesDBService(client: TMDbClient) =

  let mapSearchMovie (m: SearchMovie) =
    client.GetMovieAsync(m.Id, MovieMethods.Credits)
    |> Async.AwaitTask

  let mapMovie (m: Movie) =
    let director =
      m.Credits.Crew
      |> Seq.find (fun c -> c.Job = "Director")

    { Id = m.Id.ToString()
      Title = m.Title
      Synopsys = m.Overview
      Rate = Rate m.Popularity
      ReleaseDate = if m.ReleaseDate.HasValue then DateOnly.FromDateTime(m.ReleaseDate.Value) else DateOnly.MinValue
      Director =
        { Name = director.Name
          FirstName = director.Name.Split(" ").[0] }
      Actors =
        m.Credits.Cast
        |> Seq.map
             (fun c ->
               { Name = c.Name
                 FirstName = c.Name.Split(" ").[0] })
        |> List.ofSeq }

  interface IMoviesProvider with
    member this.GetPopulars() =
      async {
        let! movies =
          client.GetMoviePopularListAsync("en", 1)
          |> Async.AwaitTask

        return!
          movies.Results
          |> Seq.map mapSearchMovie
          |> Async.Parallel
      }
      |> Async.RunSynchronously
      |> Seq.map mapMovie
      |> List.ofSeq

    member this.GetUpcoming() =
      async {
        let! movies =
          client.GetMovieUpcomingListAsync("en", 1)
          |> Async.AwaitTask

        return!
          movies.Results
          |> Seq.map mapSearchMovie
          |> Async.Parallel
      }
      |> Async.RunSynchronously
      |> Seq.map mapMovie
      |> List.ofSeq

    member this.GetById(id: string) =
      let success, _id = Int32.TryParse(id)

      if not success then
        raise (InvalidCastException($"Cannot parse id '{id}' to int"))

      let movie =
        client.GetMovieAsync(_id, MovieMethods.Credits)
        |> Async.AwaitTask
        |> Async.RunSynchronously
        |> mapMovie

      Some movie
