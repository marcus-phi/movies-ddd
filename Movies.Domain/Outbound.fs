module Movies.Domain.Outbound

open Movies.Domain.Models

type IMoviesProvider =
  abstract member GetPopulars : unit -> Movie list
  abstract member GetUpcoming : unit -> Movie list
  abstract member GetById : id: string -> Movie option