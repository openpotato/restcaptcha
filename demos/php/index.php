<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>RESTCaptcha Demo</title>
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.7/dist/css/bootstrap.min.css" integrity="sha256-zRgmWB5PK4CvTx4FiXsxbHaYRBBjz/rvu97sOC7kzXI=" crossorigin="anonymous">
  <script src="http://localhost:65189/restcaptcha.js"></script>
</head>

<body class="bg-light">

  <div class="container py-5">
    <div class="row justify-content-center">
      <div class="col-md-6">
        <h1 class="h4 text-center mb-4">RESTCaptcha Demo</h2>
        <div class="card shadow">
          <div class="card-body">
            <h2 class="card-title text-center mb-4">Login</h2>

            <form id="restcaptcha-form" method="POST" action="verify-login.php">

              <div class="mb-3">
                <label for="username" class="form-label">Username</label>
                <input id="username" type="text" name="username" class="form-control" required>
              </div>

              <div class="mb-3">
                <label for="password" class="form-label">Password</label>
                <input id="password" type="password" name="password" class="form-control" required>
              </div>

              <!-- CAPTCHA status callout -->
              <div id="restcaptcha-callout">
                  <div id="restcaptcha-check" class="alert alert-light d-flex align-items-center" role="alert">
                    <input id="restcaptcha-trigger" type="checkbox" class="form-check-input me-2">
                     <label class="form-check-label" for="restcaptcha-trigger">
                        I'm not a robot
                    </label>
                  </div>
                  <div id="restcaptcha-solving" class="alert alert-light d-flex align-items-center d-none" role="alert">
                    <div class="spinner-border spinner-border-sm me-2" role="status" aria-hidden="true"></div>
                    <div>Verifying and solving challenge...</div>
                  </div>
                  <div id="restcaptcha-success" class="alert alert-success d-flex align-items-center d-none" role="alert">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-check-circle me-2" viewBox="0 0 16 16">
                      <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0m-3.97-3.03a.75.75 0 0 0-1.08.022L7.477 9.417 5.384 7.323a.75.75 0 0 0-1.06 1.06L6.97 11.03a.75.75 0 0 0 1.079-.02l3.992-4.99a.75.75 0 0 0-.01-1.05z"/>
                    </svg>
                    <div>I'm a human!</div>
                  </div>
                  <div id="restcaptcha-failed" class="alert alert-warning d-flex align-items-center d-none" role="alert">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-check-circle me-2" viewBox="0 0 16 16">
                      <path d="M9.05.435c-.58-.58-1.52-.58-2.1 0L.436 6.95c-.58.58-.58 1.519 0 2.098l6.516 6.516c.58.58 1.519.58 2.098 0l6.516-6.516c.58-.58.58-1.519 0-2.098zM8 4c.535 0 .954.462.9.995l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 4.995A.905.905 0 0 1 8 4m.002 6a1 1 0 1 1 0 2 1 1 0 0 1 0-2"/>
                    </svg>
                    <div id="restcaptcha-failed-msg">Failed</div>
                  </div>
                  <div id="restcaptcha-error" class="alert alert-danger d-flex align-items-center d-none" role="alert">
                    <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" class="bi bi-check-circle me-2" viewBox="0 0 16 16">
                      <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0M5.354 4.646a.5.5 0 1 0-.708.708L7.293 8l-2.647 2.646a.5.5 0 0 0 .708.708L8 8.707l2.646 2.647a.5.5 0 0 0 .708-.708L8.707 8l2.647-2.646a.5.5 0 0 0-.708-.708L8 7.293z"/>
                    </svg>
                    <div id="restcaptcha-error-msg">Error</div>
                  </div>
              </div>

              <button id="loginButton" type="submit" class="btn btn-primary w-100" disabled>Login</button>

            </form>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- CAPTCHA challenge -->
  <script type="module">

    const captcha = new RestCaptcha("http://localhost:65189/v1/challenge", "restcaptcha-form");

    captcha.onStarted = () => {
      document.getElementById("restcaptcha-check")?.classList.add("d-none");
      document.getElementById("restcaptcha-solving")?.classList.remove("d-none");
    };

    captcha.onSolved = () => {
      document.getElementById("restcaptcha-solving")?.classList.add("d-none");
      document.getElementById("restcaptcha-success")?.classList.remove("d-none");
    };

    captcha.onFailed = (msg) => {
      document.getElementById("restcaptcha-solving")?.classList.add("d-none");
      document.getElementById("restcaptcha-failed")?.classList.remove("d-none");
      document.getElementById("restcaptcha-failed-msg").textContent = "Failed: " + msg;
    };

    captcha.onError = (msg) => {
      document.getElementById("restcaptcha-solving")?.classList.add("d-none");
      document.getElementById("restcaptcha-error")?.classList.remove("d-none");
      document.getElementById("restcaptcha-error-msg").textContent = errorMsg;
    };

    document.getElementById("restcaptcha-trigger")?.addEventListener("click", async () => { await captcha.solve(); });

  </script>

  <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.7/dist/js/bootstrap.min.js" integrity="sha256-lel57Jim1/CW4I1iEkaHexK/J9h/ZRneeORKiQuNOIg=" crossorigin="anonymous"></script>
  
</body>

</html>
