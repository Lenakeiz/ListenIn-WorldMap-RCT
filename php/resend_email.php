<?php
require 'conn.php'; 

$email_hash = $_POST["email_hash"];
$email = $_POST["email"];

//ASK FOR ALREADY USED EMAIL
$preparedStatement = dbConnection::get()->prepare('SELECT * FROM user WHERE email_hash = :email_hash');

$preparedStatement->execute(array('email_hash' => $email_hash));

if($preparedStatement->rowCount() > 0)
{
	$to      = $email; // Send email to our user
	$subject = "ListenIn - Signup | Verification"; // Give the email a subject 
	
	$message = "Thanks for signing up!" . "\r\n";
	$message .= "Your account has been created, you can login with the following credentials after you have activated your account by pressing the url below." . "\r\n\n";
	$message .= "------------------------" . "\r\n";
	$message .= "Mail: " . $email . "\r\n";
	$message .= "------------------------" . "\r\n\n";
	$message .= "Please click this link to activate your account:" . "\r\n";
	$message .= "https://listeninsoftv.ucl.ac.uk/php/verify.php?hash_e=" . $email_hash . "\r\n\n";
	$message = nl2br($message);
						 
	$headers = "MIME-Version: 1.0" . "\r\n";
	$headers .= "Content-type:text/html;charset=UTF-8" . "\r\n";
	$headers .= 'From: <noreply@ucl.ac.uk>' . "\r\n";

	mail($to, $subject, $message, $headers);

	echo "bien";
}
else
{
	echo "mal";
}
			
?>
