<?php
require 'conn.php'; 

$email_hash = $_POST["email_hash"];

//ASK FOR THE SITES
$preparedStatement = dbConnection::get()->prepare('SELECT * FROM user WHERE email_hash = :email_hash LIMIT 1');

$preparedStatement->execute(array('email_hash' => $email_hash));

if($preparedStatement->rowCount() > 0)
{
	$row = $preparedStatement->fetch();
	
	if($row['email_verified'] == 'false')
	{
		echo 'email_not_verified';
	}
	else
	{
		echo $row['password'] . ' ' . $row['id'] . ' ' . $row['has_logged'];
	}
}
else
{
	echo 'no_user';
}
			
?>
