import "./App.css";
import { useEffect, useState } from "react";

function App() {
  const [view, setView] = useState(
    localStorage.getItem("token") !== null ? "main" : "login"
  );

  if (view === "login") {
    return <LoginView setView={setView} />;
  } else if (view === "register") {
    return <RegisterView setView={setView} />;
  } else {
    return <LoggedInView setView={setView} />;
  }
}

function LoginView({ setView }) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [registerEmail, setRegisterEmail] = useState("");
  const [registerPassword, setRegisterPassword] = useState("");

  const login = () => {
    fetch("http://localhost:5113/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        email,
        password,
      }),
    })
      .then((res) => res.json())
      .then((res) => {
        localStorage.setItem("token", res.accessToken);
        setView("main");
      });
  };

  const register = () => {
    fetch("http://localhost:5113/register", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        email: registerEmail,
        password: registerPassword,
      }),
    })
      .then((response) => {
        setView("login");
        if (!response.ok) {
          throw new Error("Registration failed");
        }
        // Registrering lyckades, kan göra något här om du vill
        console.log("Registration successful");
      })
      .catch((error) => {
        // Fel vid registrering, hantera fel här
        console.error("Registration error:", error);
        // Visa lämpligt meddelande för användaren
        alert(
          "Registration failed. Please try again." +
            registerEmail +
            registerPassword
        );
      });
  };

  return (
    <>
      <div className="loginbox">
        <h1>Login</h1>
        <label>Email</label>
        <input
          value={email}
          onChange={(event) => setEmail(event.target.value)}
        />
        <br />
        <label>Password</label>
        <input
          value={password}
          onChange={(event) => setPassword(event.target.value)}
        />
        <br />
        <button onClick={login}>Login</button>
      </div>

      <div className="registerbox">
        <h1>Register</h1>
        <label>Email</label>
        <input
          value={registerEmail}
          onChange={(event) => setRegisterEmail(event.target.value)}
        />
        <br />
        <label>Password</label>
        <input
          value={registerPassword}
          onChange={(event) => setRegisterPassword(event.target.value)}
        />
        <br />
        <button onClick={register}>Register</button>
      </div>
    </>
  );
}

function LoggedInView({ setView }) {
  const [movieName, setMovieName] = useState("");
  const [movieDescription, setMovieDescription] = useState("");
  const [movies, setMovies] = useState([]);
  const [deleteMovie, setDeleteMovie] = useState("");

  useEffect(() => {
    fetch("http://localhost:5113/api/movie/movies", {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + localStorage.getItem("token"),
      },
    })
      .then((res) => res.json())
      .then(setMovies);
  }, []);

  const createMovie = () => {
    fetch("http://localhost:5113/api/movie", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + localStorage.getItem("token"),
      },
      body: JSON.stringify({
        title: movieName,
        description: movieDescription,
      }),
    })
      .then((res) => {
        if (res.status === 409) {
          alert("A movie with that title already exists.");
        } else if (res.status === 400) {
          alert("You must enter a proper title and description.");
        } else {
          res.json().then((movie) => {
            setMovies([...movies, movie]);
          });
        }
      })
      .catch((err) => {
        console.log(Object.keys(err));
        console.log(err);
      });
  };

  const removeMovie = () => {
    fetch("http://localhost:5113/api/movie/movie/" + movieName, {
      method: "DELETE",
      headers: {
        "Content-Type": "application/json",
        Authorization: "Bearer " + localStorage.getItem("token"),
      },
    })
      .then((res) => {
        if (res.status === 409) {
          alert("A movie with that title already exists.");
        } else if (res.status === 400) {
          alert("You must enter a proper title and description.");
        } else {
          // Ta bort filmen från listan när borttagningen är lyckad
          setMovies(movies.filter((movie) => movie.title !== movieName));
        }
      })
      .catch((err) => {
        console.error("Error removing movie:", err);
      });
  };

  const changeMovie = (updateMovie, newSeenStatus) => {
    fetch(
      `http://localhost:5113/api/movie/movie/${updateMovie}?completed=${newSeenStatus}`,
      {
        method: "PUT",
        headers: {
          "Content-Type": "application/json",
          Authorization: "Bearer " + localStorage.getItem("token"),
        },
      }
    )
      .then((res) => {
        if (res.status === 409) {
          alert("A movie with that title already exists.");
        } else if (res.status === 400) {
          alert("You must enter a proper title and description.");
        } else if (res.ok) {
          // Kontrollera att begäran gick igenom
          // Uppdatera den lokala listan av filmer baserat på det uppdaterade filmobjektet
          setMovies((prevMovies) =>
            prevMovies.map((movie) =>
              movie.title === updateMovie
                ? { ...movie, seen: newSeenStatus }
                : movie
            )
          );
        }
      })
      .catch((err) => {
        console.error("Error removing movie:", err);
      });
  };

  return (
    <div className="App">
      <button
        onClick={() => {
          localStorage.removeItem("token");
          setView("login");
        }}
      >
        Logout
      </button>
      <div className="bigcontainer">
        <div className="container">
          <div className="box">
            {" "}
            {/* Create */}
            <h2>Create movie</h2>
            <label>Name</label>
            <input
              value={movieName}
              onChange={(event) => setMovieName(event.target.value)}
            />
            <br />
            <label>Description</label>
            <input
              value={movieDescription}
              onChange={(event) => setMovieDescription(event.target.value)}
            />
            <br />
            <button onClick={createMovie}>Create</button>
          </div>

          <div className="box">
            {/* Delete */}
            <h2>Delete Movie</h2>
            <label htmlFor="">Title</label>
            <input
              value={deleteMovie}
              onChange={(event) => setDeleteMovie(event.target.value)}
            />

            <br />
            <button onClick={removeMovie}>Remove</button>
          </div>
        </div>
        <div className="container">
          <h2>Movies</h2>
          <ul>
            {movies.map((movie) => (
              <Movie
                key={movie.id}
                id={movie.id}
                title={movie.title}
                description={movie.description}
                seen={movie.seen}
                onChangeStatus={(newSeenStatus) =>
                  changeMovie(movie.title, newSeenStatus)
                }
              />
            ))}
          </ul>
        </div>
      </div>
    </div>
  );
}

function Movie({ id, title, description, seen, onChangeStatus }) {
  const changeStatus = () => {
    onChangeStatus(!seen);
  };
  return (
    <>
      <h3>{title}</h3>
      <span>Title: </span>
      <span>{title}</span>
      <br />
      <span>Description: </span>
      <span>{description}</span>
      <br />
      <span>Seen status: </span>
      <span>{seen ? "Seen" : "Unseen"}</span>
      <br />
      <button onClick={changeStatus}>Change status</button>
    </>
  );
}

function RegisterView() {}

export default App;
