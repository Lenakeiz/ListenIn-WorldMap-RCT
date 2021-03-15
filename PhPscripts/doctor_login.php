<?php
include 'connection.php'; 

$username = $_POST["username"];
$password = $_POST["password"];
$result = "";

//ASK FOR THE SITES
$preparedStatement = $dbConnection->prepare('SELECT * FROM account WHERE username = :username and password = :password');

$preparedStatement->execute(array('username' => $username, 'password' => $password));

if($preparedStatement->rowCount() > 0)
{
	$result = 1;
}
else
{
	$result = 0;
}

echo $result;

?>