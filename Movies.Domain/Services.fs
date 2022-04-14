module Movies.Domain.Services

open Movies.Domain.Errors
open Movies.Domain.Models
open Movies.Domain.Outbound

type IMoviesService =
  abstract member GetPopulars : unit -> Movie list
  abstract member GetUpcoming : unit -> Movie list
  abstract member GetById : id: string -> Movie

type MoviesServiceImpl(moviesProvider: IMoviesProvider) =

  interface IMoviesService with
    member this.GetPopulars() =
      let movies = moviesProvider.GetPopulars()

      if movies.IsEmpty then
        raise (MoviesNotFound("Popular movies not found"))
      else
        movies

    member this.GetUpcoming() =
      let movies = moviesProvider.GetUpcoming()

      if movies.IsEmpty then
        raise (MoviesNotFound("Latest movies not found"))
      else
        movies

    member this.GetById(id: string) =
      let movie = moviesProvider.GetById id

      match movie with
      | Some m -> m
      | None -> raise (MoviesNotFound($"Movie with id {id} not found"))
