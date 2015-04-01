  <?php
$u = $_GET['u'];
$servername = "localhost";
$username = "root";
$password = "Group5";
$dbname = "Creechky";

// Create connection
$conn = new mysqli($servername, $username, $password, $dbname);
// Check connection
if ($conn->connect_error) {
     die("Connection failed: " . $conn->connect_error);
} 

$sql = "SELECT UID, Name, Phone_number, FaceFront, Restriction_type FROM user where UID = '".$u."'";
$result = $conn->query($sql);
$image = -1;
$decoded_image=base64_decode($image);
echo "<form>";


if ($result->num_rows > 0) {
     // output data of each row
         while($row = $result->fetch_assoc()) {
		$image = $row['FaceFront'];
         echo '<img src="data:image/jpeg;base64,'.base64_encode($image) .'" />';
		echo " <br /> <a href='#HomePage'> <button type='button' onclick='Delete(this.id)' id='{$row['UID']}'>Delete</button></a> <br /> 
		Name: <br /><input type='text' id='Name' value='{$row['Name']}'><br /> Phone Number:<br /> <input type='text' id='Phone_number' value='{$row['Phone_number']}'><br />
		 Restriction Type: <br /><input type='text' id='Restriction_type' value='{$row['Restriction_type']}'><br /><a href='#HomePage'><button type='button' onclick='Submit(this.id)' id='{$row['UID']}'>Submit</button></a>";
     } 
} else {
     echo "0 results";
}
echo "</table></form>";
$conn->close();
?>  