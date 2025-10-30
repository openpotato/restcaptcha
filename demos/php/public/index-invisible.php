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
  <title>RESTCaptcha PHP Invisible Mode Demo</title>
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.7/dist/css/bootstrap.min.css" integrity="sha256-zRgmWB5PK4CvTx4FiXsxbHaYRBBjz/rvu97sOC7kzXI=" crossorigin="anonymous">

  <!-- Load restcaptcha.js script -->
  <script src="<?= htmlspecialchars($config['url_jsclient']) ?>"></script>
  
</head>
<body class="bg-light">
  <main role="main" class="container py-5">
    <div class="row justify-content-center">
      <div class="col-md-6">
        <h2 class="h4 text-center mb-4">RESTCaptcha Invisible Mode Demo</h2>
        <p>
          This login screen is for demonstration purposes only. You can enter any username and/or password.
        </p>
        <div class="card shadow">
          <div class="card-body">
            <h2 class="card-title text-center mb-4">Login</h2>

            <!-- This is our form -->
            <form id="restcaptcha-form" method="POST" action="verify-login.php">

              <div class="mb-3">
                <label for="username" class="form-label">Username</label>
                <input id="username" type="text" name="username" class="form-control" required>
              </div>

              <div class="mb-3">
                <label for="password" class="form-label">Password</label>
                <input id="password" type="password" name="password" class="form-control" required>
              </div>

              <!-- RESTCaptcha widget (auto mode) -->
              <div 
                id="restcaptcha-widget" 
                data-api-baseurl="<?= htmlspecialchars($config['api_base_url']) ?>"
                data-sitekey="<?= htmlspecialchars($config['site_key']) ?>"
                data-widget-mode="invisible" 
                data-widget-language="en"
                data-widget-css-failed="alert alert-warning"
                data-widget-css-failed-body="d-flex align-items-center"
                data-widget-css-failed-icon="me-2"
                data-widget-css-error="alert alert-danger"
                data-widget-css-error-body="d-flex align-items-center"
                data-widget-css-error-icon="me-2"
                data-widget-css-footer="d-flex justify-content-end align-items-center mt-3 mb-1 gap-2"
                data-widget-css-links="d-flex gap-2"
                data-widget-css-link-external="link-secondary link-offset-1"
                data-widget-css-link-reset="link-offset-1"
                data-widget-external-link1-href="<?= htmlspecialchars($config['url_about']) ?>"
                data-widget-external-link1-text="About RESTCaptcha"
                data-callback-solved="onSolved"
                data-callback-reset="onReset"
                >
              </div>

              <!-- Our login button -->
              <button id="loginButton" type="submit" class="btn btn-primary w-100" disabled>Login</button>

            </form>
          </div>
        </div>
        <p class="d-flex justify-content-center align-items-center gap-2 mt-4">
          <a href="index.php">Interactive mode</a>
          <a href="index-auto.php">Auto mode</a> 
          <a href="index-headless.php">Headless mode</a>
        </p>
        <p class="d-flex justify-content-center align-items-center mt-4">
            <small>Made with <a href="https://www.php.net/">PHP</a>. Source code on <a href="https://github.com/openpotato/restcaptcha">GitHub</a>.</small>
        </p>
      </div>
    </div>
  </main>

  <script>
    
    // Enable login button   
    function onSolved(token, solution) {
      document.getElementById("loginButton").disabled = false;
    }
    
    // Disable login button   
    function onReset(token) {
      document.getElementById("loginButton").disabled = true;
    }

  </script>

  <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.7/dist/js/bootstrap.min.js" integrity="sha256-lel57Jim1/CW4I1iEkaHexK/J9h/ZRneeORKiQuNOIg=" crossorigin="anonymous"></script>
  
</body>
</html>
