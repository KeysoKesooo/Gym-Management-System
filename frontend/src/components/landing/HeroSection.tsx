export default function HeroSection() {
  return (
    <section className="bg-gray-100 min-h-screen flex items-center">
      <div className="container mx-auto px-6 md:px-20 flex flex-col-reverse md:flex-row items-center">
        <div className="md:w-1/2 text-center md:text-left">
          <h1 className="text-5xl font-bold text-gray-800 mb-4">
            Get Fit, Stay Healthy
          </h1>
          <p className="text-gray-600 mb-6">
            Join our gym today and achieve your fitness goals with expert trainers and modern equipment.
          </p>
        </div>
        <div className="md:w-1/2">
          <img
            src="/gym-hero.png"
            alt="Gym Hero"
            className="w-full rounded-xl shadow-lg"
          />
        </div>
      </div>
    </section>
  );
}
