# Inizializzazione di pygame
import pygame
import sys
import subprocess
import os

pygame.init()

# Caricamento dell'immagine
background_image = pygame.image.load("/Users/angela.plescia/Desktop/Biomedical Project/Body parts.jpg")  # Sostituisci con il percorso della tua immagine

# Creazione della finestra che si adatta alle dimensioni dello schermo
screen_info = pygame.display.Info()
screen_width = screen_info.current_w
screen_height = screen_info.current_h
screen = pygame.display.set_mode((screen_width, screen_height), pygame.RESIZABLE)
pygame.display.set_caption("Corpo Umano a Cartone")


# Definizione del colore e della posizione dei bottoni
play_button_color = (144, 238, 144)  # Verde chiaro
exit_button_color = (255, 100, 100)  # Rosso chiaro
light_pink = (255, 182, 193)  # Rosa chiaro
dark_pink = (255, 20, 147)  # Rosa scuro
aqua_green = (127, 255, 212)  # Verde acqua
dark_aqua_green = (0, 128, 128)  # Verde acqua scuro
light_blue = (173, 216, 230)  # Blu chiaro
dark_blue = (0, 0, 139)  # Blu scuro
light_green = (144, 238, 144)  # Verde chiaro
dark_green = (0, 100, 0)  # Verde scuro
skin_pink = (255, 218, 185)  # Rosa pelle
dark_skin_pink = (205, 133, 63)  # Rosa pelle scuro
light_red = (255, 100, 100)  # Rosso chiaro
dark_red = (139, 0, 0)  # Rosso scuro
light_orange = (255, 255, 153) # Arancione chiaro
dark_orange = (255, 140, 0) # Arancione scuro
light_brown = (210, 180, 140) # Marrone chiaro
dark_brown = (139, 69, 19) # Marrone scuro
light_black = (105, 105, 105) # Nero chiaro
dark_black = (0, 0, 0) # Nero scuro
light_maroon = (176, 48, 96) # Bordeaux chiaro
dark_maroon = (128, 0, 0) # Bordeaux scuro


play_button_rect = pygame.Rect(screen_width - 220, screen_height - 60, 100, 50)  # Posizione e dimensioni del bottone "PLAY"
exit_button_rect = pygame.Rect(screen_width - 110, screen_height - 60, 100, 50)  # Posizione e dimensioni del bottone "EXIT"

# Font per il testo dei bottoni
font = pygame.font.Font(None, 36)
exit_button_text = font.render("EXIT", True, (200, 0, 0))  # Scritta in rosso scuro

# Font per il testo grande a sinistra
large_font = pygame.font.Font(None, 144)  # Font molto più grande
body_text = large_font.render("BODY", True, (255, 165, 0))  # Testo in arancione
parts_text = large_font.render("PARTS", True, (255, 165, 0))  # Testo in arancione

# Font per il testo  a destra
large_font = pygame.font.Font(None, 50)  # Font molto più grande
body_text2 = large_font.render("Go ahead", True, (176, 48, 96)) # Bordeaux chiaro
parts_text2 = large_font.render("Click and learn!", True, (176, 48, 96)) # Bordeaux chiaro

# Tempo di ritardo in millisecondi
delay_time = 3000  # 3 secondi

# Funzione per avviare il gioco (Head)
def start_game_head():
    script_path = os.path.join(os.path.dirname(__file__), "head_first_quiz.py")
    if os.path.exists(script_path):
        subprocess.Popen([sys.executable, script_path])
    else:
        print(f"Path {script_path} not found.")

# Funzione per avviare il gioco (Hair)
def start_game_hair():
    script_path = os.path.join(os.path.dirname(__file__), "hair_first_quiz.py")
    if os.path.exists(script_path):
        subprocess.Popen([sys.executable, script_path])
    else:
        print(f"Path {script_path} not found.")


# Funzione per avviare il gioco (Shoulder)
def start_game_shoulder():
    script_path = os.path.join(os.path.dirname(__file__), "shoulder_first_quiz.py")
    if os.path.exists(script_path):
        subprocess.Popen([sys.executable, script_path])
    else:
        print(f"Path {script_path} not found.")

# Funzione per avviare il gioco (Eye)
def start_game_eye():
    script_path = os.path.join(os.path.dirname(__file__), "eye_first_quiz.py")
    if os.path.exists(script_path):
        subprocess.Popen([sys.executable, script_path])
    else:
        print(f"Path {script_path} not found.")

# Funzione per avviare il gioco (Hear)
def start_game_ear():
    script_path = os.path.join(os.path.dirname(__file__), "ear_first_quiz.py")
    if os.path.exists(script_path):
        subprocess.Popen([sys.executable, script_path])
    else:
        print(f"Path {script_path} not found.")

# Funzione per avviare il gioco (Arm)
def start_game_arm():
    script_path = os.path.join(os.path.dirname(__file__), "arm_first_quiz.py")
    if os.path.exists(script_path):
        subprocess.Popen([sys.executable, script_path])
    else:
        print(f"Path {script_path} not found.")

# Funzione per avviare il gioco (Finger)
def start_game_finger():
    script_path = os.path.join(os.path.dirname(__file__), "finger_first_quiz.py")
    if os.path.exists(script_path):
        subprocess.Popen([sys.executable, script_path])
    else:
        print(f"Path {script_path} not found.")

# Funzione per avviare il gioco (Hand)
def start_game_hand():
    script_path = os.path.join(os.path.dirname(__file__), "hand_first_quiz.py")
    if os.path.exists(script_path):
        subprocess.Popen([sys.executable, script_path])
    else:
        print(f"Path {script_path} not found.")

# Funzione per avviare il gioco (Knee)
def start_game_knee():
    script_path = os.path.join(os.path.dirname(__file__), "knee_first_quiz.py")
    if os.path.exists(script_path):
        subprocess.Popen([sys.executable, script_path])
    else:
        print(f"Path {script_path} not found.")

# Funzione per avviare il gioco (Knee)
def start_game_foot():
    script_path = os.path.join(os.path.dirname(__file__), "foot_first_quiz.py")
    if os.path.exists(script_path):
        subprocess.Popen([sys.executable, script_path])
    else:
        print(f"Path {script_path} not found.")

    # Funzione per avviare il gioco (Knee)

def start_game_leg():
    script_path = os.path.join(os.path.dirname(__file__), "leg_first_quiz.py")
    if os.path.exists(script_path):
        subprocess.Popen([sys.executable, script_path])
    else:
        print(f"Path {script_path} not found.")

# Funzione per avviare il gioco (Knee)
def start_game_toe():
    script_path = os.path.join(os.path.dirname(__file__), "toe_first_quiz.py")
    if os.path.exists(script_path):
        subprocess.Popen([sys.executable, script_path])
    else:
        print(f"Path {script_path} not found.")



# Loop principale
running = True
start_time = pygame.time.get_ticks()
while running:
    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False
        elif event.type == pygame.MOUSEBUTTONDOWN:
            if exit_button_rect.collidepoint(event.pos):
                running = False
            elif play_button_rect.collidepoint(event.pos):
                # Aggiungi qui il codice per la funzionalità di PLAY
                print("Play button clicked")

    # Riempimento dello sfondo con un colore giallo chiaro
    screen.fill((255, 255, 204))  # Giallo chiaro

    # Ridimensionamento e posizionamento dell'immagine a destra al centro, allargata in verticale
    scaled_image = pygame.transform.scale(background_image, (screen_width // 2, screen_height * 3 // 4))
    image_rect = scaled_image.get_rect()
    image_rect.midright = (screen_width - 10, screen_height // 2)
    screen.blit(scaled_image, image_rect.topleft)

    # Disegno del testo grande a sinistra
    screen.blit(body_text, (50, screen_height // 2 - body_text.get_height()))
    screen.blit(parts_text, (50, screen_height // 2))

    # Disegno del testo a destra dopo il ritardo
    current_time = pygame.time.get_ticks()
    if current_time - start_time > delay_time:
        screen.blit(body_text2, ((screen_width // 2) + 300, 15))
        screen.blit(parts_text2, ((screen_width // 2) + 250, 55))

    # Disegno dei bottoni con bordi tondeggianti
    pygame.draw.rect(screen, exit_button_color, exit_button_rect, border_radius=10)
    screen.blit(exit_button_text, (exit_button_rect.x + (exit_button_rect.width - exit_button_text.get_width()) // 2, exit_button_rect.y + (exit_button_rect.height - exit_button_text.get_height()) // 2))

    # Creazione bottoni rappresentanti parti del corpo
    head_button_rect = pygame.Rect(screen_width - 200, 115, 170, 60)
    eye_button_rect = pygame.Rect(screen_width - 200, 225, 170, 60)
    ear_button_rect = pygame.Rect(screen_width - 190, 315, 170, 60)
    finger_button_rect = pygame.Rect(screen_width - 180, 470, 170, 60)
    knee_button_rect = pygame.Rect(screen_width - 220, 575, 170, 60)
    toe_button_rect = pygame.Rect(screen_width - 190, 670, 170, 60)
    hair_button_rect = pygame.Rect(screen_width - 750, 117, 170, 60)  # Bottone "Hair" spostato di molto a sinistra
    shoulder_button_rect = pygame.Rect(screen_width - 770, 310, 200, 60)
    arm_button_rect = pygame.Rect(screen_width - 770, 380, 170, 60)  # Bottone "Arm"
    hand_button_rect = pygame.Rect(screen_width - 780, 455, 150, 60)  # Bottone "Hand"
    leg_button_rect = pygame.Rect(screen_width - 740, 540, 170, 60)  # Bottone "Leg"
    foot_button_rect = pygame.Rect(screen_width - 750, 630, 170, 60)  # Bottone "Foot"


    head_button_text = font.render("Head", True, dark_green)  # Scritta in verde scuro
    eye_button_text = font.render("Eye", True, dark_pink)  # Scritta in rosa scuro
    ear_button_text = font.render("Ear", True, dark_aqua_green)  # Scritta in verde acqua scuro
    finger_button_text = font.render("Finger", True, dark_blue)  # Scritta in blu scuro
    knee_button_text = font.render("Knee", True, dark_green)  # Scritta in verde scuro
    toe_button_text = font.render("Toe", True, dark_skin_pink)  # Scritta in rosa pelle scuro
    hair_button_text = font.render("Hair", True, dark_red)  # Scritta in rosso scuro
    shoulder_button_text = font.render("Shoulder", True, dark_blue)  # Scritta in blu scuro
    arm_button_text = font.render("Arm", True, dark_orange)  # Scritta in arancione scuro
    hand_button_text = font.render("Hand", True, dark_brown)  # Scritta in marrone scuro
    leg_button_text = font.render("Leg", True, dark_black)  # Scritta in nero scuro
    foot_button_text = font.render("Foot", True, dark_maroon)  # Scritta in bordeaux scuro

    pygame.draw.rect(screen, play_button_color, head_button_rect, border_radius=10)
    screen.blit(head_button_text, (head_button_rect.x + (head_button_rect.width - head_button_text.get_width()) // 2, head_button_rect.y + (head_button_rect.height - head_button_text.get_height()) // 2))
    pygame.draw.rect(screen, light_pink, eye_button_rect, border_radius=10)
    screen.blit(eye_button_text, (eye_button_rect.x + (eye_button_rect.width - eye_button_text.get_width()) // 2, eye_button_rect.y + (eye_button_rect.height - eye_button_text.get_height()) // 2))
    pygame.draw.rect(screen, aqua_green, ear_button_rect, border_radius=10)
    screen.blit(ear_button_text, (ear_button_rect.x + (ear_button_rect.width - ear_button_text.get_width()) // 2, ear_button_rect.y + (ear_button_rect.height - ear_button_text.get_height()) // 2))
    pygame.draw.rect(screen, light_blue, finger_button_rect, border_radius=10)
    screen.blit(finger_button_text, (finger_button_rect.x + (finger_button_rect.width - finger_button_text.get_width()) // 2, finger_button_rect.y + (finger_button_rect.height - finger_button_text.get_height()) // 2))
    pygame.draw.rect(screen, light_green, knee_button_rect, border_radius=10)
    screen.blit(knee_button_text, (knee_button_rect.x + (knee_button_rect.width - knee_button_text.get_width()) // 2, knee_button_rect.y + (knee_button_rect.height - knee_button_text.get_height()) // 2))
    pygame.draw.rect(screen, skin_pink, toe_button_rect, border_radius=10)
    screen.blit(toe_button_text, (toe_button_rect.x + (toe_button_rect.width - toe_button_text.get_width()) // 2, toe_button_rect.y + (toe_button_rect.height - toe_button_text.get_height()) // 2))
    pygame.draw.rect(screen, light_red, hair_button_rect, border_radius=10)
    screen.blit(hair_button_text, (hair_button_rect.x + (hair_button_rect.width - hair_button_text.get_width()) // 2, hair_button_rect.y + (hair_button_rect.height - hair_button_text.get_height()) // 2))
    pygame.draw.rect(screen, light_blue, shoulder_button_rect, border_radius=10)
    screen.blit(shoulder_button_text, (shoulder_button_rect.x + (shoulder_button_rect.width - shoulder_button_text.get_width()) // 2, shoulder_button_rect.y + (shoulder_button_rect.height - shoulder_button_text.get_height()) // 2))
    pygame.draw.rect(screen, light_orange, arm_button_rect, border_radius=10)
    screen.blit(arm_button_text, (arm_button_rect.x + (arm_button_rect.width - arm_button_text.get_width()) // 2, arm_button_rect.y + (arm_button_rect.height - arm_button_text.get_height()) // 2))
    pygame.draw.rect(screen, light_brown, hand_button_rect, border_radius=10)
    screen.blit(hand_button_text, (hand_button_rect.x + (hand_button_rect.width - hand_button_text.get_width()) // 2, hand_button_rect.y + (hand_button_rect.height - hand_button_text.get_height()) // 2))
    pygame.draw.rect(screen, light_black, leg_button_rect, border_radius=10)
    screen.blit(leg_button_text, (leg_button_rect.x + (leg_button_rect.width - leg_button_text.get_width()) // 2, leg_button_rect.y + (leg_button_rect.height - leg_button_text.get_height()) // 2))
    pygame.draw.rect(screen, light_maroon, foot_button_rect, border_radius=10)
    screen.blit(foot_button_text, (foot_button_rect.x + (foot_button_rect.width - foot_button_text.get_width()) // 2, foot_button_rect.y + (foot_button_rect.height - foot_button_text.get_height()) // 2))

    for event in pygame.event.get():
        if event.type == pygame.QUIT:
            running = False
        elif event.type == pygame.MOUSEBUTTONDOWN:
            if head_button_rect.collidepoint(event.pos):
                start_game_head()
            elif hair_button_rect.collidepoint(event.pos):
                start_game_hair()
            elif shoulder_button_rect.collidepoint(event.pos):
                start_game_shoulder()
            elif eye_button_rect.collidepoint(event.pos):
                start_game_eye()
            elif ear_button_rect.collidepoint(event.pos):
                start_game_ear()
            elif arm_button_rect.collidepoint(event.pos):
                start_game_arm()
            elif finger_button_rect.collidepoint(event.pos):
                start_game_finger()
            elif hand_button_rect.collidepoint(event.pos):
                start_game_hand()
            elif knee_button_rect.collidepoint(event.pos):
                start_game_knee()
            elif foot_button_rect.collidepoint(event.pos):
                start_game_foot()
            elif leg_button_rect.collidepoint(event.pos):
                start_game_leg()
            elif toe_button_rect.collidepoint(event.pos):
                start_game_toe()
            elif exit_button_rect.collidepoint(event.pos):
                running = False

    # Aggiornamento dello schermo
    pygame.display.flip()

# Chiusura di pygame
pygame.quit()
sys.exit()