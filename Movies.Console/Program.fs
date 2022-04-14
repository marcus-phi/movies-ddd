open Microsoft.FSharp.Core
open Movies.Domain.Models
open Movies.Domain.Outbound
open Movies.Domain.Services
open Movies.Infra.Services
open TMDbLib.Client

let printUsage () =
  printfn "Usage: -apiKey <apiKey> <command>"
  printfn "Commands:"
  printfn "    -popular - Get popular movies"
  printfn "    -upcoming - Get upcoming movies"
  printfn "    -get <id> - Get movie by id"

let getMoviesService apiKey =
  let client = new TMDbClient(apiKey)

  let moviesProvider =
    TheMoviesDBService(client) :> IMoviesProvider

  MoviesServiceImpl(moviesProvider) :> IMoviesService
  
let printMovie (m: Movie) =
  printfn "***************************************"
  printfn $"Id: {m.Id}"
  printfn $"Title: {m.Title}"
  printfn $"Synopsys: {m.Synopsys}"
  printfn $"Rate: {m.Rate}"
  printfn $"ReleaseDate: {m.ReleaseDate}"
  printfn $"Director: {m.Director.FirstName} - {m.Director.Name}"
  printfn "Actors:"
  m.Actors |> Seq.iter (fun a -> printfn $"  * {a.FirstName} - {a.Name}")

[<EntryPoint>]
let main args =

  let arguments = List.ofArray args

  match arguments with
  | "-apiKey" :: apiKey :: "-popular" :: _ ->
    (getMoviesService apiKey).GetPopulars()
    |> Seq.iter printMovie
  | "-apiKey" :: apiKey :: "-upcoming" :: _ ->
    (getMoviesService apiKey).GetUpcoming()
    |> Seq.iter printMovie
  | "-apiKey" :: apiKey :: "-get" :: id :: _ ->
    (getMoviesService apiKey).GetById id
    |> printMovie
  | _ -> printUsage ()

  0
