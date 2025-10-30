<?php 
/**
 * RESTCaptcha PHP example 
 */

// Load configuration from .env
$root   = dirname(__DIR__);
$config = require $root . '/config/config.php'; 

?>
<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <title>RESTCaptcha PHP Headless Demo</title>
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.7/dist/css/bootstrap.min.css" integrity="sha256-zRgmWB5PK4CvTx4FiXsxbHaYRBBjz/rvu97sOC7kzXI=" crossorigin="anonymous">
  
  <!-- Load restcaptcha.js script -->
  <script src="<?= htmlspecialchars($config['url_jsclient']) ?>"></script>
  
</head>
<body class="bg-light">
  <main role="main" class="container py-5">
    <div class="row justify-content-center">
      <div class="col-md-6">
        <h2 class="h4 text-center mb-4">RESTCaptcha Headless Demo</h2>
        <p>
          This login screen is for demonstration purposes only. You can enter any username and/or password.
        </p>
        <div class="card shadow">
          <div class="card-body">
            <h2 class="card-title text-center mb-4">Login</h2>

            <!-- This is our form -->
            <form id="headless-restcaptcha-form" method="POST" action="verify-login.php">

              <div class="mb-3">
                <label for="username" class="form-label">Username</label>
                <input id="username" type="text" name="username" class="form-control" required>
              </div>

              <div class="mb-3">
                <label for="password" class="form-label">Password</label>
                <input id="password" type="password" name="password" class="form-control" required>
              </div>

              <!-- RESTCaptcha widget (headless mode, do not name it `restcaptcha-widget`) -->
              <div id="headless-restcaptcha-widget"></div>

              <!-- Our login button -->
              <button id="loginButton" type="submit" class="btn btn-primary w-100" disabled>Login</button>

            </form>
          </div>
        </div>
        <p class="d-flex justify-content-center align-items-center gap-2 mt-4">
          <a href="index.php">Interactive mode</a>
          <a href="index-auto.php">Auto mode</a> 
          <a href="index-invisible.php">Invisible mode</a>
        </p>
        <p class="d-flex justify-content-center align-items-center mt-4">
            <small>Made with <a href="https://www.php.net/">PHP</a>. Source code on <a href="https://github.com/openpotato/restcaptcha">GitHub</a>.</small>
        </p>
      </div>
    </div>
</main>

  <script>

    const widget = document.getElementById("headless-restcaptcha-widget");  
    const form = widget.closest("form");

    const apiBaseUrl = "<?= htmlspecialchars($config['api_base_url']) ?>";
    const siteKey = "<?= htmlspecialchars($config['site_key']) ?>";
    
    // Initialize RESTCaptcha
    const captcha = new HeadlessRestCaptcha(apiBaseUrl, siteKey, "en");

    // Set RESTCaptcha handlers
    captcha.onStarted = () => {
        renderSolvingUI();
    };

    captcha.onSolved = (token, solution) => {
        injectTokenAndSolution(token, solution);
        renderSolvedUI();
        document.getElementById("loginButton").disabled = false;
    };

    captcha.onFailed = (errorMessage) => {
        renderFailedUI(errorMessage);
    };

    captcha.onError = (exception) => {
        renderErrorUI(exception.message);
    };

    captcha.onReset = (token) => {
        removeTokenAndSolution();
        renderInteractiveUI();
        document.getElementById("loginButton").disabled = true;
    };

    // Start RESTCaptcha workflow
    renderInteractiveUI();

    // Renders the UI for interactive mode.
    function renderInteractiveUI() {
      widget.innerHTML = `
        <div id="restcaptcha-interactive" class="alert alert-light" role="alert">
          <div id="restcaptcha-interactive-body" class="form-check">
            <input id="restcaptcha-interactive-checkbox" type="checkbox" class="form-check-input">
            <label id="restcaptcha-interactive-checklabel" for="restcaptcha-interactive-checkbox" class="form-check-label">
              I am not a robot.
            </label>
          </div>  
        </div>
      `;

      // Start challenge when clicking the checkbox
      const checkbox = document.getElementById(`restcaptcha-interactive-checkbox`);
      checkbox?.addEventListener("click", async (event) => {
        if (event.target.checked) {
            await captcha.solve();
        }
      });
    }

    // Renders the UI for visualising the solving process.
    function renderSolvingUI() {
      widget.innerHTML = `
        <div id="restcaptcha-solving" class="alert alert-light" role="alert">
          <div id="restcaptcha-solving-body" class="d-flex align-items-center">
            <div id=restcaptcha-solving-animation" class="spinner-border spinner-border-sm me-3" role="status" aria-hidden="true"></div>
            <div id="restcaptcha-solving-text">
              Solving CAPTCHA challenge...
            </div>
          </div>
        </div>
      `;
    }

    // Renders the UI for visualising solved challenges.
    function renderSolvedUI() {
      widget.innerHTML = `
        <div id="restcaptcha-success" class="alert alert-success" role="alert">
          <div id="restcaptcha-success-body" class="d-flex align-items-center">
            <div id="restcaptcha-success-text">
              &#128512; I am a human!
            </div>
          </div>
        </div>
      `;
    }

    // Renders the UI for visualising verification errors.
    function renderFailedUI(errorMessage) {
      widget.innerHTML = `
        <div id="restcaptcha-failed" class="alert alert-warning" role="alert">
          <div id="restcaptcha-failed-body" class="d-flex align-items-center">
            <div id="restcaptcha-failed-text">
              &#128543; ${errorMessage} <a id="restcaptcha-reset-link" href="#">Try again</a>.
            </div>
          </div>
        </div>
      `;

      // Resets the CAPTCHA workflow
      const resetLink = document.getElementById(`restcaptcha-reset-link`);
      resetLink?.addEventListener("click", async (event) => {
        event.preventDefault();
        const token = getToken();
        if (token) {
            captcha.reset(token);
        } else {
            captcha.notifyOnResetEvent(null);
        }
      });
    }

    // Renders the UI for visualising errors.
    function renderErrorUI(errorMessage) {
      widget.innerHTML = `
        <div id="restcaptcha--error" class="alert alert-danger" role="alert">
          <div id="restcaptcha--error-body" class="d-flex align-items-center">
            <div id="restcaptcha-error-text">
              &#128558; ${errorMessage} <a id="restcaptcha-reset-link" href="#">Try Again</a>.
            </div>
          </div>
        </div>
      `;

      // Resets the CAPTCHA workflow
      const resetLink = document.getElementById(`restcaptcha-reset-link`);
      resetLink?.addEventListener("click", async (event) => {
          event.preventDefault();
          const token = getToken();
          if (token) {
              captcha.reset(token);
          } else {
              captcha.notifyOnResetEvent(null);
          }
      });
    }    

    // Retrieves the value of the hidden token input element for the widget.
    function getToken() {
        return document.getElementById(`restcaptcha-token`)?.value;
    }

    // Injects hidden input elements for the CAPTCHA token and solution into the parent 
    // form of the widget.
    function injectTokenAndSolution(token, solution) {
        form.insertAdjacentHTML("beforeend", `
            <input id="restcaptcha-token" type="hidden" name="captcha-token" value="${token}">
            <input id="restcaptcha-solution" type="hidden" name="captcha-solution" value="${solution}">
        `);
    }

    // Removes the hidden token and solution input elements associated with the widget.
    function removeTokenAndSolution() {
        document.getElementById(`restcaptcha-token`)?.remove();
        document.getElementById(`restcaptcha-solution`)?.remove();
    }

  </script>

  <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.7/dist/js/bootstrap.min.js" integrity="sha256-lel57Jim1/CW4I1iEkaHexK/J9h/ZRneeORKiQuNOIg=" crossorigin="anonymous"></script>
  
</body>
</html>
