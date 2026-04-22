using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfEscapeGame.Classes;
using WpfEscapeGame.Enums;

namespace WpfEscapeGame
{
    public partial class MainWindow : Window
    {
        Room currentRoom;

        public MainWindow()
        {
            InitializeComponent();

            Room room1 = new Room()
            {
                Name = "bedroom",
                Description = "I seem to be in a medium sized bedroom. " +
                              "There is a locker to the left, a nice rug on the floor, " +
                              "and a bed to the right."
            };
            room1.Image = "ss-bedroom.png";

            Room room2 = new Room()
            {
                Name = "living room",
                Description = "A cozy living room. There's a clock on the wall and a plant in the corner."
            };
            room2.Image = "ss-living.png";

            Room room3 = new Room()
            {
                Name = "computer room",
                Description = "A dark room with an old computer and a Commodore flag on the wall."
            };
            room3.Image = "ss-computer.png";

            Item key1 = new Item()
            {
                Name = "small silver key",
                Description = "A small silver key, makes me think of one I had at highschool."
            };

            Item key2 = new Item()
            {
                Name = "large key",
                Description = "A large key. Could this be my way out?"
            };

            Item bed = new Item()
            {
                Name = "bed",
                Description = "Just a bed. I am not tired now."
            };
            bed.HiddenItem = key1;
            bed.IsPortable = false;

            Item locker = new Item()
            {
                Name = "locker",
                Description = "A locker. I wonder what's inside."
            };
            locker.IsLocked = true;
            locker.Key = key1;
            locker.HiddenItem = key2;
            locker.IsPortable = false;

            Item floorMat = new Item()
            {
                Name = "floor mat",
                Description = "A bit ragged floor mat, but still one of the most popular designs."
            };

            Item chair = new Item()
            {
                Name = "chair",
                Description = "A wooden chair. Nothing special about it."
            };
            chair.IsPortable = false;

            Item poster = new Item()
            {
                Name = "poster",
                Description = "A poster of a woman on the beach. Nice!"
            };

            room1.Items.Add(floorMat);
            room1.Items.Add(bed);
            room1.Items.Add(locker);
            room1.Items.Add(chair);
            room1.Items.Add(poster);

            room2.Items.Add(new Item() { Name = "clock", Description = "A wall clock. It reads 7 o'clock." });
            room2.Items.Add(new Item() { Name = "plant", Description = "A green tropical plant." });

            room3.Items.Add(new Item() { Name = "computer", Description = "An old Commodore 64. Classic!" });
            room3.Items.Add(new Item() { Name = "trash bin", Description = "An empty trash bin." });

            Door door1 = new Door()
            {
                Name = "green door",
                Description = "A green door leading to the living room.",
                IsLocked = true,
                Key = key2,
                ToRoom = room2
            };

            Door door2 = new Door()
            {
                Name = "left door",
                Description = "An open door to the computer room.",
                IsLocked = false,
                ToRoom = room3
            };

            Door door3 = new Door()
            {
                Name = "right door",
                Description = "The door back to the living room.",
                IsLocked = false,
                ToRoom = room2
            };

            Door door4 = new Door()
            {
                Name = "exit door",
                Description = "The exit door. It's locked tight.",
                IsLocked = true,
                ToRoom = null
            };

            room1.Doors.Add(door1);
            room2.Doors.Add(door2);
            room2.Doors.Add(door4);
            room3.Doors.Add(door3);

            currentRoom = room1;
            txtRoomDesc.Text = currentRoom.Description;
            txtMessage.Text = "I am awake, but cannot remember who I am!? " +
                              "Must have been a hell of a party last night...";
            UpdateUI();
        }

        private void UpdateUI()
        {
            lstRoomItems.Items.Clear();
            foreach (Item itm in currentRoom.Items)
                lstRoomItems.Items.Add(itm);

            lstDoors.Items.Clear();
            foreach (Door d in currentRoom.Doors)
                lstDoors.Items.Add(d);

            UpdateRoomImage();
        }

        private void UpdateRoomImage()
        {
            if (currentRoom.Image != null)
            {
                imgRoom.Source = new BitmapImage(
                    new Uri("pack://application:,,,/" + currentRoom.Image, UriKind.Absolute));
            }
        }

        private void LstItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnCheck.IsEnabled = lstRoomItems.SelectedValue != null;
            btnPickUp.IsEnabled = lstRoomItems.SelectedValue != null;
            btnUseOn.IsEnabled = lstRoomItems.SelectedValue != null
                                && lstMyItems.SelectedValue != null;
            btnDrop.IsEnabled = lstMyItems.SelectedValue != null;
        }

        private void BtnCheck_Click(object sender, RoutedEventArgs e)
        {
            Item roomItem = (Item)lstRoomItems.SelectedItem;

            if (roomItem.IsLocked)
            {
                txtMessage.Text = $"{roomItem.Description}. It is firmly locked.";
                return;
            }

            Item foundItem = roomItem.HiddenItem;
            if (foundItem != null)
            {
                txtMessage.Text = $"{RandomMessageGenerator.GetRandomMessage(MessageType.Found)} It's a {foundItem.Name}!";
                lstMyItems.Items.Add(foundItem);
                roomItem.HiddenItem = null;
                return;
            }

            txtMessage.Text = roomItem.Description;
        }

        private void BtnPickUp_Click(object sender, RoutedEventArgs e)
        {
            Item selItem = (Item)lstRoomItems.SelectedItem;

            if (!selItem.IsPortable)
            {
                txtMessage.Text = $"I can't pick up the {selItem.Name}, it's too heavy!";
                return;
            }

            txtMessage.Text = $"I just picked up the {selItem.Name}.";
            lstMyItems.Items.Add(selItem);
            lstRoomItems.Items.Remove(selItem);
            currentRoom.Items.Remove(selItem);
        }

        private void BtnUseOn_Click(object sender, RoutedEventArgs e)
        {
            Item myItem = (Item)lstMyItems.SelectedItem;
            Item roomItem = (Item)lstRoomItems.SelectedItem;

            if (roomItem.Key != myItem)
            {
                txtMessage.Text = RandomMessageGenerator.GetRandomMessage(MessageType.Fail);
                return; // ✅ return was hier vergeten!
            }

            roomItem.IsLocked = false;
            roomItem.Key = null;
            lstMyItems.Items.Remove(myItem);
            txtMessage.Text = $"I just unlocked the {roomItem.Name}!";
        }

        private void BtnDrop_Click(object sender, RoutedEventArgs e)
        {
            Item selItem = (Item)lstMyItems.SelectedItem;

            txtMessage.Text = $"I dropped the {selItem.Name} back in the room.";
            currentRoom.Items.Add(selItem);
            lstMyItems.Items.Remove(selItem);
            UpdateUI();
        }

        private void LstDoors_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            btnOpenWith.IsEnabled = lstDoors.SelectedValue != null
                                 && lstMyItems.SelectedValue != null;
            btnEnter.IsEnabled = lstDoors.SelectedValue != null;
        }

        private void BtnOpenWith_Click(object sender, RoutedEventArgs e)
        {
            Door selDoor = (Door)lstDoors.SelectedItem;
            Item myItem = (Item)lstMyItems.SelectedItem;

            if (selDoor.Key != myItem)
            {
                txtMessage.Text = RandomMessageGenerator.GetRandomMessage(MessageType.Fail);
                return;
            }

            selDoor.IsLocked = false;
            selDoor.Key = null;
            lstMyItems.Items.Remove(myItem);
            txtMessage.Text = $"I just unlocked the {selDoor.Name}!";
        }

        private void BtnEnter_Click(object sender, RoutedEventArgs e)
        {
            Door selDoor = (Door)lstDoors.SelectedItem;

            if (selDoor.IsLocked)
            {
                txtMessage.Text = "This door is locked. I need a key.";
                return;
            }

            if (selDoor.ToRoom == null)
            {
                txtMessage.Text = "🎉 Congratulations! You escaped the building!";
                return;
            }

            currentRoom = selDoor.ToRoom;
            txtRoomDesc.Text = currentRoom.Description;
            txtMessage.Text = $"I entered the {currentRoom.Name}.";
            UpdateUI();
        }
    }
}