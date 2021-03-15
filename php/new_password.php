<?php
require 'conn.php'; 

$email = $_POST["eh"];
$new_password = md5($_POST["np"]);

$preparedStatement = dbConnection::get()->prepare('UPDATE user SET password = :password WHERE email_hash = :email_hash LIMIT 1');
$preparedStatement->execute(array('password' => $new_password, 'email_hash' => $email));

$to      = $_POST["e"]; // Send email to our user
$subject = "ListenIn - Password changed"; // Give the email a subject 

$message = "Your password has changed!" . "\r\n\n";
$message .= "------------------------" . "\r\n";
$message .= "Mail: " . $_POST["e"] . "\r\n";
$message .= "Password: " . $_POST["np"] . "\r\n";
$message .= "------------------------" . "\r\n\n";
$message .= "Thanks for using ListenIn!" . "\r\n";
$message = nl2br($message);
					 
$headers = "MIME-Version: 1.0" . "\r\n";
$headers .= "Content-type:text/html;charset=UTF-8" . "\r\n";
$headers .= 'From: <noreply@ucl.ac.uk>' . "\r\n";

mail($to, $subject, $message, $headers);

echo "Password changed!";
			
?>
