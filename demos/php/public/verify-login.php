<?php
/**
 * RESTCaptcha PHP example 
 */

// Import RESTCaptcha API Client classes into the global namespace
use RestCaptcha\ApiClient;
use RestCaptcha\ProblemDetailsException;
use RestCaptcha\VerifyStatus;

// Load configuration from .env
$root    = dirname(__DIR__);
$config  = require $root . '/config/config.php';       

// Init config variables
$apiBaseUrl        = $config['api_base_url'] ?? '';
$siteKey           = $config['site_key'] ?? ''; 
$siteSecret        = $config['site_secret'] ?? ''; 
$noSSLVerification = $config['no_ssl_verification'] ?? false; 

// Init form variables
$token             = $_POST['captcha-token'] ?? '';
$solution          = $_POST['captcha-solution'] ?? '';

// Ready Steady Go! 
$apiClient = new ApiClient($siteKey, $siteSecret, "de", [
    'client_options' => [
        'base_uri' => $apiBaseUrl,
        'verify'   => !$noSSLVerification, // Never disable SSL verification in production!
    ]
]);

try {
  
    // Optional for logging reasons
    $callerIp = $_SERVER['REMOTE_ADDR'] ?? ''; 

    // Verify token and solution
    $status = $apiClient->verifySolution($token, $solution, $callerIp);

    // Get username and password from form
    $username = $_POST['username'] ?? '';
    $password = $_POST['password'] ?? '';

    // Compute return URL (fallback to homepage if missing)
    $returnUrl = $_POST['returnUrl'] ?? ($_SERVER['HTTP_REFERER'] ?? '/');

    // Helper for HTML escaping
    $esc = fn(string $s) => htmlspecialchars($s, ENT_QUOTES, 'UTF-8');

    // Render the Echo page with username, password and verification error
    ?>
    <!DOCTYPE html>
    <html lang="en">
    <head>
        <meta charset="UTF-8">
        <title>RESTCaptcha Demo Echo</title>
        <meta name="viewport" content="width=device-width, initial-scale=1">
        <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.7/dist/css/bootstrap.min.css" integrity="sha256-zRgmWB5PK4CvTx4FiXsxbHaYRBBjz/rvu97sOC7kzXI=" crossorigin="anonymous">
    </head>
    <body class="bg-light">
        <main role="main" class="container py-5">
            <div class="row justify-content-center">
                <div class="col-md-6">
                    <h2 class="h4 text-center mb-4">RESTCaptcha Demo Echo</h2>
                    <div class="alert alert-warning" role="alert">
                        Below is an echo of what you entered. Do not use real credentials.
                    </div>
                    <div class="card shadow">
                        <div class="card-body">
                            <dl class="row">
                                <dt class="col-sm-4">User name</dt>
                                <dd class="col-sm-8"><?= $esc($username) ?></dd>
                                <dt class="col-sm-4">Password</dt>
                                <dd class="col-sm-8"><?= $esc($password) ?></dd>
                                <dt class="col-sm-4">Verification status</dt>
                                <dd class="col-sm-8"><?= $esc($status->value) ?></dd>
                            </dl>
                            <a href="<?= $esc($returnUrl) ?>" class="btn btn-secondary">Back</a>
                        </div>
                    </div>
                </div>
            </div>
        </main>
        <script src="https://cdn.jsdelivr.net/npm/bootstrap@5.3.7/dist/js/bootstrap.min.js" integrity="sha256-lel57Jim1/CW4I1iEkaHexK/J9h/ZRneeORKiQuNOIg=" crossorigin="anonymous"></script>
    </body>
    </html>
    <?php    

} catch (ProblemDetailsException $exception) {

    // Echo RESTCaptcha error
    http_response_code($exception->status);

    echo "<h2>RESTCaptcha error</h2>";
    echo "<ul>";
    echo "<li>Type: $exception->type</li>";
    echo "<li>Title: $exception->title</li>";
    echo "<li>Status: $exception->status</li>";
    echo "<li>Detail: $exception->detail</li>";

    if (!empty($exception->errors)) {
        echo "<li>Errors:";
        echo "<ul>";

        $lines = [];
        foreach ($exception->errors as $key => $messages) {
            if (is_array($messages)) {
                $flat = [];
                $it = new \RecursiveIteratorIterator(new \RecursiveArrayIterator($messages));
                foreach ($it as $m) {
                    $flat[] = (string)$m;
                }
                $msg = '[' . implode(', ', $flat) . ']';
            } else {
                $msg = '[' . (string)$messages . ']';
            }
            echo "<li>{$key}: {$msg}</li>";
        }

        echo "</ul>";
        echo "</li>";
    }

    echo "<li>TraceId: $exception->traceId</li>";
    echo "</ul>";

} catch (Exception $exception) {

    // Echo everything else that is wrong.
    http_response_code(500);

    echo('Unexpected error: ' . $exception->getMessage());
}
