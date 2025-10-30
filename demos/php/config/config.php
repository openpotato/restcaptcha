<?php
/**
 * RESTCaptcha PHP example 
 */

$root = dirname(__DIR__);

require $root . '/vendor/autoload.php';

$dotenv = Dotenv\Dotenv::createImmutable($root);
$dotenv->load();

return [
    'api_base_url'        => $_ENV['API_BASE_URL'] ?? '',
    'site_key'            => $_ENV['SITE_KEY'] ?? '',
    'site_secret'         => $_ENV['SITE_SECRET'] ?? '',
    'url_jsclient'        => $_ENV['URL_JSCLIENT'] ?? '',
    'url_about'           => $_ENV['URL_ABOUT'] ?? '',
    'url_privacy'         => $_ENV['URL_PRIVACY'] ?? '',
    'no_ssl_verification' => $_ENV['NO_SSL_VERIFICATION'] ?? false
];
?>
