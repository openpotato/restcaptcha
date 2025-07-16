<?php
require_once 'verify-captcha.php';

if ($_SERVER['REQUEST_METHOD'] !== 'POST') {
  http_response_code(405);
  exit("Method not allowed");
}

try {
  verify_captcha($_POST);
} catch (CaptchaVerificationException $ex) {
  exit($ex->getMessage());
}

$username = $_POST['username'] ?? '';
$password = $_POST['password'] ?? '';

// Credential check
if ($username === "admin" && $password === "12345") {
  echo "<h2>Login successful</h2>";
} else {
  echo "<h3>Invalid username or password</h3>";
}
?>