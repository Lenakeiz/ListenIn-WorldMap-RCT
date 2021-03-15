<?php
require 'conn.php'; 

$id_user = $_POST["id_user"];
$code = $_POST["code"];

$preparedStatement = dbConnection::get()->prepare('SELECT * FROM redeem_code WHERE code = :code LIMIT 1');

$preparedStatement->execute(array('code' => $code));

if($preparedStatement->rowCount() > 0)
{
	$row = $preparedStatement->fetch();
	
	if($row['id_user_assigned'] == "")
	{
		$preparedStatement = dbConnection::get()->prepare('UPDATE redeem_code SET id_user_assigned = :id_user WHERE code = :code LIMIT 1');
		$preparedStatement->execute(array('id_user' => $id_user, 'code' => $code));
		echo "bien";
	}
	else
	{
		echo "in_use";
	}
}
else
{
	echo "code_not_exist";
}
			
?>
