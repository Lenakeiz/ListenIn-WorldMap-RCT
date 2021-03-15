<?php
$id_user = $_POST['id_user'];
$file = $_POST['file_name'];

$file_route = '../files/'.$id_user.'/'.$file;

$checksumPHP = crc32($_POST['content']);
$checksumC = $_POST['checksum'];
if($checksumC == $checksumPHP)
{
	//check for file
	if (!file_exists($file_route))
	{
		echo "empty";
		return;
	}
	else
	{
		$file_content = $file."#".file_get_contents($file_route);
		echo $file_content;
		return;
	}
}
else
{
	echo "checksum_error";
}

?>
