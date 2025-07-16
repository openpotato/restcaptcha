<?php

class CaptchaVerificationException extends Exception {}

function verify_captcha(array $data): void
{
  $verifyData = json_encode([
    'nonce'          => $data['nonce']          ?? '',
    'nonceSignature' => $data['nonceSignature'] ?? '',
    'timeStamp'      => (int)($data['timeStamp'] ?? 0),
    'solution'       => $data['solution']       ?? '',
    'fingerprint'    => $data['fingerprint']    ?? ''
  ]);

  $ch = curl_init("https://localhost:44303/v1/verify");
  curl_setopt_array($ch, [
    CURLOPT_RETURNTRANSFER => true,
    CURLOPT_POST           => true,
    CURLOPT_HTTPHEADER     => ['Content-Type: application/json'],
    CURLOPT_POSTFIELDS     => $verifyData,
	CURLOPT_SSL_VERIFYPEER => false,
	CURLOPT_VERBOSE        => true
  ]);

  
  $response = curl_exec($ch);
  $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
  $error    = curl_error($ch);
  curl_close($ch);

  if ($httpCode !== 200) {
    throw new CaptchaVerificationException(
      "Verification request failed: HTTP $httpCode" . ($error ? " - $error" : '')
    );
  }
}
